namespace Walrus.CLI.Commands
{
    using System;
    using System.Collections.Generic;
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.Linq;
    using Core;
    using Microsoft.Extensions.Logging;

    /// <summary>
    ///     Query all Git repositories
    /// </summary>
    internal class QueryCommand : BaseCommand
    {
        private readonly ILogger _logger;

        public QueryCommand(IWalrusService walrus, ILogger<QueryCommand> logger) : base(walrus)
        {
            _logger = logger;

            Command.AddOption(new Option<DateTime>(
                new[] {"--after", "-a"},
                () => DateTime.Now.AddDays(-7),
                "Return commits on or after this date. Defaults to one week ago."));

            Command.AddOption(new Option<DateTime>(
                new[] {"--before", "-b"},
                () => DateTime.Now.AddDays(1),
                "Return commits before this date. Defaults to tomorrow."));

            // All branches is slow so no short alias will be provided
            Command.AddOption(new Option(
                "--all-branches",
                "Include all *local* branches in search"
            ));

            Command.AddOption(new Option<string>(
                new[] {"--author-email", "-e"},
                "Return commits from this author. Takes precendence over --author-alias."));

            Command.AddOption(new Option<string>(
                new[] {"--author-alias", "-u"},
                "Return commits from any alias of this author"));

            Command.AddOption(new Option<string>(
                new[] {"--repo-name", "-r"},
                "Return commits from this repository. Case insensitive."));

            Command.AddOption(new Option<WalrusQuery.QueryGrouping>(
                new[] {"--group-by", "-g"},
                "Result grouping method"
            ));

            Command.AddOption(new Option(
                new []{"--current-directory", "-c"},
                "Ignore configuration file roots and scan relative to current directory"
                ));

            Command.Handler = CommandHandler.Create((WalrusQuery query) =>
            {
                query.AddConfiguration(walrus.Config);

                HandleQuery(query);
            });
        }

        /// <inheritdoc />
        public override string Name => "query";

        /// <inheritdoc />
        public override string Description => "Query all git repositories";

        /// <summary>
        ///     Execute Git query
        /// </summary>
        /// <param name="query">Query to execute</param>
        private void HandleQuery(WalrusQuery query)
        {
            _logger.LogDebug("HandleQuery: {Query}", query);

            var commits = Walrus.ExecuteQuery(query);

            var commitCount = PrintTable(commits, query);

            Console.WriteLine(new string('=', Console.WindowWidth / 2));
            Console.WriteLine("Total Commits: {0}", commitCount);
            Console.WriteLine(new string('=', Console.WindowWidth / 2));
        }


        /// <summary>
        ///     Rough table format printer for commit list
        ///     Commits are sorted by repo then by day, a summary
        ///     SHA and commit title are shown for each commit.
        /// </summary>
        /// <param name="commits">Commits to print</param>
        /// <param name="query">Query parameter</param>
        /// <returns>Count of commits in commit</returns>
        private static int PrintTable(IEnumerable<CommitGroup> commits, WalrusQuery query)
        {
            var count = query.GroupBy switch
            {
                WalrusQuery.QueryGrouping.Repo => PrintByRepo(commits),
                WalrusQuery.QueryGrouping.Date => PrintByDate(commits),
                WalrusQuery.QueryGrouping.Author => PrintByAuthor(commits),

                _ => throw new ArgumentOutOfRangeException(nameof(query.GroupBy))
            };

            return count;
        }

        /// <summary>
        ///     Print repos to console grouped by repo name
        /// </summary>
        private static int PrintByRepo(IEnumerable<CommitGroup> commits)
        {
            var count = 0;
            var header = new string('-', Console.WindowWidth / 2);

            foreach (var repoGrouping in commits)
            {
                // file:// make a ctrl+clickable link in supported terminals
                Console.WriteLine($"Repository: {repoGrouping.Key} [file://{repoGrouping.Data.First().RepoPath}]");
                Console.WriteLine(header);

                foreach (var dateGrouping in repoGrouping.Data.GroupBy(g => g.Timestamp.Date))
                {
                    Console.WriteLine($"{dateGrouping.Key:yyyy-MM-dd}: {dateGrouping.Count()} Commits");

                    foreach (var commit in dateGrouping)
                    {
                        Console.WriteLine(
                            $"\t{commit.Timestamp:HH:mm} {commit.Sha.Substring(0, 7)} [{commit.AuthorEmail}] {commit.Message}");
                        ++count;
                    }
                }

                Console.WriteLine(Environment.NewLine);
            }

            return count;
        }

        /// <summary>
        ///     Print repos to console grouped by commit date
        /// </summary>
        private static int PrintByDate(IEnumerable<CommitGroup> commits)
        {
            var count = 0;
            var header = new string('-', Console.WindowWidth / 2);

            foreach (var dateGrouping in commits)
            {
                Console.WriteLine($"Date: {dateGrouping.Key:yyyy-MM-dd}");
                Console.WriteLine(header);

                foreach (var commit in dateGrouping.Data)
                {
                    Console.WriteLine(
                        $"\t{commit.Timestamp:HH:mm} [{commit.RepoName}] {commit.Sha.Substring(0, 7)} [{commit.AuthorEmail}] {commit.Message}");
                    ++count;
                }

                Console.WriteLine(Environment.NewLine);
            }

            return count;
        }

        /// <summary>
        ///     Print repos to console grouped by author email
        /// </summary>
        private static int PrintByAuthor(IEnumerable<CommitGroup> commits)
        {
            var count = 0;
            var header = new string('-', Console.WindowWidth / 2);

            foreach (var authorGrouping in commits)
            {
                Console.WriteLine($"Author: {authorGrouping.Key}");
                Console.WriteLine(header);

                foreach (var commit in authorGrouping.Data)
                {
                    Console.WriteLine(
                        $"\t{commit.Timestamp:yyyy-MM-dd HH:mm} [{commit.RepoName}] {commit.Sha.Substring(0, 7)} {commit.Message}");
                    ++count;
                }

                Console.WriteLine(Environment.NewLine);
            }

            return count;
        }
    }
}