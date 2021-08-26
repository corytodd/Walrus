namespace Walrus.CLI
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using System;
    using System.CommandLine;
    using System.CommandLine.Builder;
    using System.CommandLine.Parsing;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging.Abstractions;
    using Walrus.Core;

    class Program
    {
        /// <summary>
        /// Entry point
        /// </summary>
        static async Task<int> Main(string[] args)
        {
            try
            {
                var serviceProvider = ConfigureServices();

                var parser = BuildParser(serviceProvider);

                return await parser.InvokeAsync(args).ConfigureAwait(false);
            }
            catch (FileNotFoundException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("ERROR: ");
                Console.ResetColor();
                Console.WriteLine("Unable to find configuration file. " + Environment.NewLine +
                                  "You can provide a configuration file in one of two ways: " + Environment.NewLine +
                                  "1) Define an env named WALRUS_CONFIG_FILE that points to your configuration file" +
                                  Environment.NewLine +
                                  "2) Create a file named walrus.config in your current working directory" +
                                  Environment.NewLine);
                Console.WriteLine($"The original exception was: {ex.Message}");

                return 1;
            }
            catch (InvalidDataException ex)
            {               
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("ERROR: ");
                Console.ResetColor();
                Console.WriteLine("Your configuration file invalid JSON");
                Console.WriteLine($"The original exception was: {ex.Message}");
                return 1;
            }
        }
        
        /// <summary>
        /// Build command line parser utility to auto-discover all commands in this assembly
        /// </summary>
        /// <param name="serviceProvider">Hosting service</param>
        /// <returns>Command line parser</returns>
        private static Parser BuildParser(ServiceProvider serviceProvider)
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

            return commandLineBuilder.UseDefaults().Build();
        }

        /// <summary>
        /// Setup DI and services
        /// </summary>
        /// <returns></returns>
        private static ServiceProvider ConfigureServices()
        {
            var configFile = Environment.GetEnvironmentVariable("WALRUS_CONFIG_FILE") ?? "walrus.json";
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(configFile, optional: true)
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
                });

            services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton(p => p.GetRequiredService<IConfiguration>().Get<WalrusConfig>() ??
                                              WalrusConfig.Default);
            services.AddTransient<IWalrusService, WalrusService>();

            services.AddCliCommands();

            var provider = services.BuildServiceProvider();

            provider.AddWalrusLogging();

            return provider;
        }
    }
}