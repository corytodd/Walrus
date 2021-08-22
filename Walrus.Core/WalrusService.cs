namespace Walrus.Core
{
    using Microsoft.Extensions.Logging;
    using System.Collections.Generic;
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
    }
}
