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

            Command.Handler = CommandHandler.Create((QueryParams queryParams) =>
            {
                HandleQuery(queryParams);
            });
        }

        public override string Name => "query";

        public override string Description => "Query all git repositories";

        private void HandleQuery(QueryParams queryParams)
        {
            _logger.LogDebug("HandleQuery: {queryParams}", queryParams);

            var commits = Walrus
                .GetRepositories()
                .Select(r => r.GetCommitsInRangeAllBranches(queryParams.After, queryParams.Before))
                .SelectMany(c => c)
                .OrderBy(c => c.Timestamp);

            var count = commits.Count();

            Console.WriteLine("Found {0} commits", count);
        }

        private class QueryParams
        {
            public DateTime After { get; set; }

            public DateTime Before { get; set; }

            public override string ToString()
            {
                return $"After={After}, Before={Before}";
            }
        }
    }
}
