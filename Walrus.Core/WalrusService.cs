namespace Walrus.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Internal;
    using Microsoft.Extensions.Logging;
    using Repository;

    /// <summary>
    ///     Default Walrus service implementation provides access to Git repositories
    /// </summary>
    public class WalrusService : IWalrusService
    {
        private readonly ILogger _logger;
        private readonly IRepositoryProvider _repositoryProvider;

        /// <summary>
        ///     Create a new WalrusService
        /// </summary>
        /// <param name="logger">Logging context</param>
        /// <param name="repositoryProvider">Repository provider</param>
        /// <param name="config">Service configuration</param>
        public WalrusService(ILogger<WalrusService> logger, IRepositoryProvider repositoryProvider,
            IWalrusConfig config)
        {
            Ensure.IsNotNull(nameof(logger), logger);
            Ensure.IsNotNull(nameof(repositoryProvider), repositoryProvider);
            Ensure.IsNotNull(nameof(config), config);

            _logger = logger;
            _repositoryProvider = repositoryProvider;
            Config = config;
        }

        /// <inheritdoc />
        public IWalrusConfig Config { get; }

        /// <inheritdoc />
        public IEnumerable<CommitGroup> QueryCommits(WalrusQuery query)
        {
            Ensure.IsNotNull(nameof(query), query);

            var preparedQuery = new PreparedWalrusQuery(query, Config);

            var commits =
                QueryRepositories(query)
                    .AsParallel()
                    .SelectMany(r => r.Commits.Where(preparedQuery.IsMatch));

            // Wrap GroupBy in a CommitGroup so we can have keys of different types
            var grouped = query.GroupBy switch
            {
                WalrusQuery.QueryGrouping.Repo => commits
                    .GroupBy(c => c.RepoName)
                    .Select(g => new CommitGroup(g.Key,
                        g.OrderBy(c => c.Timestamp))),

                WalrusQuery.QueryGrouping.Date => commits
                    .OrderBy(c => c.Timestamp)
                    .GroupBy(c => c.Timestamp.Date)
                    .Select(g => new CommitGroup(g.Key,
                        g.OrderBy(c => c.Timestamp))),

                WalrusQuery.QueryGrouping.Author => commits
                    .GroupBy(c => c.AuthorEmail)
                    .Select(g => new CommitGroup(g.Key,
                        g.OrderBy(c => c.Timestamp))),

                _ => throw new ArgumentOutOfRangeException(nameof(query))
            };

            return grouped;
        }


        /// <inheritdoc />
        public IEnumerable<WalrusRepository> QueryRepositories(WalrusQuery? query = null)
        {
            query ??= new WalrusQuery();
            var preparedQuery = new PreparedWalrusQuery(query, Config);
            return QueryRepositories(preparedQuery);
        }

        /// <summary>
        ///     Executes prepared query to fetch all matching repositories
        /// </summary>
        /// <param name="query">Query to execute</param>
        /// <returns>List of repositories satisfying query</returns>
        private IEnumerable<WalrusRepository> QueryRepositories(PreparedWalrusQuery query)
        {
            if (query.SearchPaths is null)
            {
                _logger.LogWarning("No repository search paths were specified");

                yield break;
            }

            foreach (var root in query.SearchPaths)
            {
                var repositories = _repositoryProvider
                    .GetRepositories(root, Config.DirectoryScanDepth, query.AllBranches)
                    .Where(query.IsMatch);

                foreach (var repo in repositories)
                {
                    yield return repo;
                }
            }
        }
    }
}