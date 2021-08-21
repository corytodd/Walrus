namespace Walrus.Core
{
    using Microsoft.Extensions.Logging;
    using System.Collections.Generic;
    using Walrus.Core.Internal;

    public class WalrusService : IWalrusService
    {
        private ILogger _logger;

        public WalrusService(ILogger<WalrusConfig> logger, WalrusConfig config)
        {
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
                yield break;
            }

            foreach (var root in Config.RepositoryRoots)
            {
                var directories = Utilities.EnumerateDirectoriesToDepth(root, Config.DirectoryScanDepth);

                foreach (var repo in Utilities.GetValidRepositories(directories))
                {
                    yield return repo;
                }
            }
        }
    }
}
