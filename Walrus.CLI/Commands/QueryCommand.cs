namespace Walrus.CLI.Commands
{
    using Microsoft.Extensions.Logging;
    using System;
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

            Command.Handler = CommandHandler.Create((WalrusQuery query) =>
            {
                HandleQuery(query);
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
        private void HandleQuery(WalrusQuery query)
        {
            _logger.LogDebug("HandleQuery: {query}", query);

            var commits = Walrus
                .GetRepositories()
                .Where(r => string.IsNullOrEmpty(query.RepoName) || r.RepositoryName.Equals(query.RepoName, StringComparison.CurrentCultureIgnoreCase))
                .AsParallel()
                .Select(r => r.GetCommits(query))
                .SelectMany(c => c)
                .OrderBy(c => c.Timestamp);

            var count = commits.Count();

            Console.WriteLine("Found {0} commits", count);
        }
    }
}
