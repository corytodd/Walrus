﻿namespace Walrus.CLI.Commands
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
                "Return commits from this author. Takes precendence over --author-alias."));

            Command.AddOption(new Option<string>(
                "--author-alias",
                "Return commits from any alias of this author"));

            Command.AddOption(new Option<string>(
                "--repo-name",
                "Return commits from this repository. Case insensitive."));

            Command.AddOption(new Option(
                "--print-table",
                "Print results in a tree format"
                ));

            Command.Handler = CommandHandler.Create((WalrusQuery query, bool printTable) =>
            {
                query.AddConfiguration(walrus.Config);

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
                .Where(r => FilterRepo(r, query))
                .AsParallel()
                .Select(r => r.GetCommits(query))
                .SelectMany(c => c)
                .OrderBy(c => c.Timestamp)
                .AsEnumerable();

            // Avoid calling Count() on the commits enumerable because it is slow
            int? commitCount = null;
            if (printTable)
            {
                commitCount = PrintTable(commits);
            }

            commitCount ??= commits.Count();

            Console.WriteLine(new string('=', Console.WindowWidth / 2));
            Console.WriteLine("Total Commits: {0}", commitCount);
            Console.WriteLine(new string('=', Console.WindowWidth / 2));
        }

        /// <summary>
        /// Returns true if repository should be included in query
        /// </summary>
        /// <param name="repository">Repository to test</param>
        /// <param name="query">Query parameters</param>
        /// <returns>True if filter should be included</returns>
        private static bool FilterRepo(WalrusRepository repository, WalrusQuery query)
        {
            return string.IsNullOrEmpty(query.RepoName) || repository.RepositoryName.Equals(query.RepoName, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        ///     Rough table format printer for commit list
        ///     Commits are sorted by repo then by day, a summary 
        ///     SHA and commit title are shown for each commit.
        /// </summary>
        /// <param name="commits">Commits to print</param>
        /// <returns>Count of commits in commit</returns>
        private static int PrintTable(IEnumerable<WalrusCommit> commits)
        {
            var count = 0;
            var header = new string('-', Console.WindowWidth / 2);

            foreach (var groupRepo in commits.GroupBy(c => c.RepoName))
            {
                // file:// make a ctrl+clickable link in supported terminals
                Console.WriteLine($"Repository: {groupRepo.Key} [file://{groupRepo.First().RepoPath}]");
                Console.WriteLine(header);

                foreach (var groupDay in groupRepo.GroupBy(g => g.Timestamp.Date))
                {
                    Console.WriteLine($"{groupDay.Key:yyyy-MM-dd}: {groupDay.Count()} Commits");

                    foreach (var commit in groupDay)
                    {
                        Console.WriteLine($"\t{commit.Timestamp:HH:mm} {commit.Sha.Substring(0, 7)} [{commit.AuthorEmail}] {commit.Message}");
                        ++count;
                    }
                }
                Console.WriteLine(Environment.NewLine);
            }

            return count;
        }
    }
}
