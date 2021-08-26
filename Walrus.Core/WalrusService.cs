namespace Walrus.Core
{
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Walrus.Core.Internal;

    /// <summary>
    /// Default Walrus service implementation provides access to Git repositories
    /// </summary>
    public class WalrusService : IWalrusService
    {
        private readonly ILogger _logger;
        private readonly IRepositoryProvider _repositoryProvider;

        /// <summary>
        /// Create a new WalrusService
        /// </summary>
        /// <param name="logger">Logging context</param>
        /// <param name="repositoryProvider">Repository provider</param>
        /// <param name="config">Service configuration</param>
        public WalrusService(ILogger<WalrusService> logger, IRepositoryProvider repositoryProvider, IWalrusConfig config)
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
        public IEnumerable<CommitGroup> ExecuteQuery(WalrusQuery query)
        {
            Ensure.IsNotNull(nameof(query), query);

            var searchRoot = query.CurrentDirectory ? Environment.CurrentDirectory : null;

            var commits =
                GetAllRepositories(searchRoot, query.AllBranches)
                    .Where(r => FilterRepo(r, query))
                    .AsParallel()
                    .SelectMany(r => r.Commits.Where(query.IsMatch));

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

                _ => throw new ArgumentOutOfRangeException(nameof(query.GroupBy))
            };

            return grouped;
        }


        /// <inheritdoc />
        public IEnumerable<WalrusRepository> GetAllRepositories(string? rootDirectory = null, bool allBranches = false)
        {
            var searchPaths = string.IsNullOrEmpty(rootDirectory) ? Config.RepositoryRoots : new[] {rootDirectory};

            if (searchPaths is null)
            {
                _logger.LogWarning("No repository search paths were specified");

                yield break;
            }

            foreach (var root in searchPaths)
            {
                foreach (var repo in _repositoryProvider.GetRepositories(root, Config.DirectoryScanDepth, allBranches))
                {
                    yield return repo;
                }
            }
        }

        /// <summary>
        /// Returns true if repository should be included in query
        /// </summary>
        /// <param name="repository">Repository to test</param>
        /// <param name="query">Query parameters</param>
        /// <returns>True if filter should be included</returns>
        private static bool FilterRepo(WalrusRepository repository, WalrusQuery query)
        {
            var b = string.IsNullOrEmpty(query.RepoName) ||
                   repository.RepositoryName.Equals(query.RepoName, StringComparison.CurrentCultureIgnoreCase);
            return b;
        }
    }
}