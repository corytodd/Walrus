﻿namespace Walrus.CLI
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using System;
    using System.CommandLine;
    using System.CommandLine.Builder;
    using System.CommandLine.Parsing;
    using System.Threading.Tasks;
    using Walrus.Core;

    class Program
    {
        /// <summary>
        /// Entry point
        /// </summary>
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
            var configFile = Environment.GetEnvironmentVariable("WALRUS_CONFIG_FILE") ?? "walrus.json";
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(configFile)
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

            services.AddCliCommands();

            var provider = services.BuildServiceProvider();

            provider.AddWalrusLogging();

            return provider;
        }
    }
}
