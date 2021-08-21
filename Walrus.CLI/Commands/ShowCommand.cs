namespace Walrus.CLI.Commands
{
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.Text.Json;
    using Walrus.Core;

    /// <summary>
    /// Shows information about the specified Walrus feature
    /// </summary>
    class ShowCommand : BaseCommand
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Create a new Show command
        /// </summary>
        /// <param name="walrus">Walrus service</param>
        /// <param name="logger">Logging service</param>
        public ShowCommand(IWalrusService walrus, ILogger<ShowCommand> logger) : base(walrus)
        {
            _logger = logger;

            var subCommands = new List<Command>
            {
                new Command("config", "Show configuration")
                {
                    Handler = CommandHandler.Create(() =>
                    {
                        ShowConfig();
                    })
                },
                new Command("repos", "Show known repositories")
                {
                    Handler = CommandHandler.Create(() =>
                    {
                        ShowRepositories();
                    })
                },
            };

            foreach (var subCommand in subCommands)
            {
                Command.AddCommand(subCommand);
            }
        }

        /// <inheritdoc />
        public override string Name => "show";

        /// <inheritdoc />
        public override string Description => "Display information";

        /// <summary>
        /// Show the active configuration JSON
        /// </summary>
        private void ShowConfig()
        {
            _logger.LogDebug("ShowConfig");

            var options = new JsonSerializerOptions { WriteIndented = true };
            var configJson = JsonSerializer.Serialize(Walrus.Config, options);

            Console.WriteLine(configJson);
        }

        /// <summary>
        /// Show all known repositories
        /// </summary>
        private void ShowRepositories()
        {
            _logger.LogDebug("ShowRepositories");

            var repos = Walrus.GetRepositories();
            var count = 0;
            foreach (var repo in repos)
            {
                ++count;
                Console.WriteLine(repo.RepositoryPath);
            }

            Console.WriteLine("Found {0} repos", count);
        }
    }
}
