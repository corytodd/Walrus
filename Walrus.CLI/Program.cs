namespace Walrus.CLI
{
    using System;
    using System.CommandLine;
    using System.CommandLine.Builder;
    using System.CommandLine.Parsing;
    using System.IO;
    using System.Threading.Tasks;
    using Core;
    using Core.Repository;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    internal class Program
    {
        /// <summary>
        ///     Entry point
        /// </summary>
        private static async Task<int> Main(string[] args)
        {
            try
            {
                var serviceProvider = ConfigureServices();

                var parser = BuildParser(serviceProvider);

                return await parser
                    .InvokeAsync(args)
                    .ConfigureAwait(false);
            }
            catch(WalrusConfigurationException ex)
            {
                WriteError("Your walrus config file is invalid", ex);
                return -1;
            }
        }

        /// <summary>
        /// Write error message to console in red
        /// </summary>
        /// <param name="message">Text to show</param>
        /// <param name="ex">Original exception</param>
        private static void WriteError(string message, Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("ERROR: ");
            Console.ResetColor();
            Console.WriteLine(message);
            if(ex is not null)
            {
                Console.WriteLine($"Details: {ex.Message}");
            }
        }

        /// <summary>
        ///     Build command line parser utility to auto-discover all commands in this assembly
        /// </summary>
        /// <param name="serviceProvider">Hosting service</param>
        /// <returns>Command line parser</returns>
        private static Parser BuildParser(IServiceProvider serviceProvider)
        {
            var rootCommand = new RootCommand
            {
                Description = "Walrus Git Utility"
            };

            var commandLineBuilder = new CommandLineBuilder(rootCommand);

            // Add each command instance as a subcommand (verb) to the root command
            foreach (var command in serviceProvider.GetServices<IComposableCommand>())
            {
                commandLineBuilder.AddCommand(command.Command);
            }

            return commandLineBuilder.UseDefaults()
                .UseExceptionHandler((e, context) =>
                {
                    // Unwind to inner most exception
                    var innerEx = e;
                    while(innerEx.InnerException is not null){
                        innerEx = innerEx.InnerException;
                    }

                    switch(innerEx)
                    {
                        case DirectoryNotFoundException dnfe:
                            WriteError("One or more directories in your config file do not exist", dnfe);
                            break;

                        case FileNotFoundException fnfe:
                            var message = "Unable to find configuration file. " + Environment.NewLine +
                                          "You can provide a configuration file in one of two ways: " + Environment.NewLine +
                                          "1) Define an env named WALRUS_CONFIG_FILE that points to your configuration file" +
                                          Environment.NewLine +
                                          "2) Create a file named walrus.config in your current working directory" +
                                          Environment.NewLine;
                            WriteError(message, fnfe);
                            break;
                        case InvalidDataException ide:
                            WriteError("Your configuration file invalid JSON", ide);
                            break;
                        case Exception ex:
                            WriteError("Exception", ex);
                            break;
                        default:
                            WriteError("Unknown Error", null);
                            break;
                    }
                })
                .Build();
        }

        /// <summary>
        ///     Setup DI and services
        /// </summary>
        /// <returns></returns>
        private static IServiceProvider ConfigureServices()
        {
            var configFile = Environment.GetEnvironmentVariable("WALRUS_CONFIG_FILE") ?? "walrus.json";
            configFile = PathHelper.ResolvePath(configFile);
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(configFile, true)
                .Build();

            var services = new ServiceCollection()
                .AddLogging(configure =>
                {
#if DEBUG
                    configure.SetMinimumLevel(LogLevel.Debug);
                    configure.AddDebug();
#else
                    configure.SetMinimumLevel(LogLevel.Error);
#endif
                    configure.AddConsole();
                })
                .AddSingleton<IConfiguration>(configuration)
                .AddSingleton<IWalrusConfig>(p => {
                    // Load and validation user configuration
                    var config = p.GetRequiredService<IConfiguration>().Get<WalrusConfig>() ??
                                                  WalrusConfig.Default;
                    config.ValidateOrThrow();
                    return config;
                })
                .AddTransient<IRepositoryProvider, RepositoryProvider>()
                .AddTransient<IWalrusService, WalrusService>()
                .AddCliCommands();

            return services.BuildServiceProvider()
                .AddWalrusLogging();
        }
    }
}