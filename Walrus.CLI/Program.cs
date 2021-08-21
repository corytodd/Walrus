using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Walrus.Core;

namespace Walrus.CLI
{
    class Program
    {
        private static readonly Lazy<Assembly> ThisAssembly = new(() => typeof(Program).Assembly);

        static async Task<int> Main(string[] args)
        {
            var serviceProvider = ConfigureServices();

            Parser parser = BuildParser(serviceProvider);

            return await parser.InvokeAsync(args).ConfigureAwait(false);
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
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("walrus.json")
                .Build();

            var services = new ServiceCollection()
                    .AddLogging(configure =>
                    {
                        configure.SetMinimumLevel(LogLevel.Debug);
                        configure.AddDebug();
                        configure.AddConsole();
                    });
            
            services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton(provider =>
            {
                return provider.GetRequiredService<IConfiguration>().Get<WalrusConfig>();
            });
            services.AddTransient<IWalrusService, WalrusService>();

            // All command should dervice from BaseCommand but we'll accept the base interface as well
            var commandType = typeof(IComposableCommand);
            var commands = ThisAssembly.Value
                .GetTypes()
                .Where(x => !x.IsAbstract && commandType.IsAssignableFrom(x));

            foreach (var command in commands)
            {
                services.AddSingleton(commandType, command);
            }

            return services.BuildServiceProvider();
        }
    }
}
