namespace Walrus.Core
{
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Walrus.Core.Internal;

    /// <summary>
    /// Default Walrus service implementation provides access to Git repositories
    /// </summary>
    public class WalrusService : IWalrusService
    {
        private ILogger _logger;

        /// <summary>
        /// Create a new WalrusService
        /// </summary>
        /// <param name="logger">Logging context</param>
        /// <param name="config">Service configuration</param>
        public WalrusService(ILogger<WalrusConfig> logger, WalrusConfig config)
        {
            Ensure.IsNotNull(nameof(logger), logger);
            Ensure.IsNotNull(nameof(config), config);

            _logger = logger;
            Config = config;
        }

        /// <inheritdoc />
        public WalrusConfig Config { get; private set; }

        /// <inheritdoc />
        public IEnumerable<CommitGroup> ExecuteQuery(WalrusQuery query)
        {
            var commits =
                GetRepositories()
                    .Where(r => FilterRepo(r, query))
                    .AsParallel()
                    .Select(r => r.GetCommits(query))
                    .SelectMany(c => c);

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
        public IEnumerable<WalrusRepository> GetRepositories()
        {
            if (Config.RepositoryRoots is null)
            {
                _logger.LogWarning("No repository roots are configured. Check your configuration file.");

                yield break;
            }

            foreach (var root in Config.RepositoryRoots)
            {
                var directories = Utilities.EnumerateDirectoriesToDepth(root, Config.DirectoryScanDepth);

                foreach (var repo in Utilities.GetValidRepositories(directories))
                {
                    var repository = new WalrusRepository(repo);
                    yield return repository;
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
            return string.IsNullOrEmpty(query.RepoName) ||
                   repository.RepositoryName.Equals(query.RepoName, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}