namespace Walrus.CLI.Commands
{
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.Linq;
    using Walrus.Core;

    /// <summary>
    /// Query all Git repositories
    /// </summary>
    class QueryCommand : BaseCommand
    {
        private readonly ILogger _logger;

        public QueryCommand(IWalrusService walrus, ILogger<QueryCommand> logger) : base(walrus)
        {
            _logger = logger;

            Command.AddOption(new Option<DateTime>(
                "--after",
                () => DateTime.Now.AddDays(-7),
                "Return commits on or after this date. Defaults to one week ago."));

            Command.AddOption(new Option<DateTime>(
                "--before",
                () => DateTime.Now.AddDays(1),
                "Return commits before this date. Defaults to tomorrow."));

            Command.AddOption(new Option(
                "--all-branches",
                "Include all *local* branches in search"
                ));

            Command.AddOption(new Option<string>(
                "--author-email",
                "Return commits from this author"));

            Command.AddOption(new Option<string>(
                "--repo-name",
                "Return commits from this repository. Case insensitive."));

            Command.AddOption(new Option(
                "--print-table",
                "Print results in a tree format"
                ));

            Command.Handler = CommandHandler.Create((WalrusQuery query, bool printTable) =>
            {
                HandleQuery(query, printTable);
            });
        }

        /// <inheritdoc />
        public override string Name => "query";

        /// <inheritdoc />
        public override string Description => "Query all git repositories";

        /// <summary>
        /// Execute Git query
        /// </summary>
        /// <param name="query">Query to execute</param>
        /// <param name="printTable">True to print results as table</param>
        private void HandleQuery(WalrusQuery query, bool printTable)
        {
            _logger.LogDebug("HandleQuery: {query}", query);

            var commits = Walrus
                .GetRepositories()
                .Where(r => string.IsNullOrEmpty(query.RepoName) || r.RepositoryName.Equals(query.RepoName, StringComparison.CurrentCultureIgnoreCase))
                .AsParallel()
                .Select(r => r.GetCommits(query))
                .SelectMany(c => c)
                .OrderBy(c => c.Timestamp)
                .AsEnumerable();

            if (printTable)
            {
                PrintTable(commits);
            }
        }

        /// <summary>
        ///     Rough table format printer for commit list
        ///     Commits are sorted by repo then by day, a summary 
        ///     SHA and commit title are shown for each commit.
        /// </summary>
        /// <param name="commits">Commits to print</param>
        private void PrintTable(IEnumerable<WalrusCommit> commits)
        {
            var header = new string('-', Console.WindowWidth / 2);

            foreach (var groupRepo in commits.GroupBy(c => c.RepoName))
            {
                Console.WriteLine($"Repository: {groupRepo.Key}");
                Console.WriteLine(header);

                foreach (var groupDay in groupRepo.GroupBy(g => g.Timestamp.Date))
                {
                    Console.WriteLine($"{groupDay.Key:d}: {groupDay.Count()} Commits");

                    foreach(var commit in groupDay)
                    {
                        Console.WriteLine($"\t{commit.Sha} {commit.Message}");
                    }
                }
                Console.WriteLine();
            }

            var count = commits.Count();

            Console.WriteLine(new string('=', Console.WindowWidth / 2));
            Console.WriteLine("Total Commits: {0}", count);
        }
    }
}
