namespace Walrus.Core
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    ///     Walrus service configuration
    /// </summary>
    public sealed class WalrusConfig : IWalrusConfig
    {
        /// <summary>
        ///     Generate a default configuration
        /// </summary>
        public static WalrusConfig Default => new()
        {
            DirectoryScanDepth = 3
        };

        /// <inheritdoc />
        public int DirectoryScanDepth { get; set; }

        /// <inheritdoc />
        public IList<string> RepositoryRoots { get; set; } = new List<string>();

        /// <inheritdoc />
        public IDictionary<string, IList<string>>? AuthorAliases { get; set; }

        /// <inheritdoc />
        public void ValidateOrThrow()
        {
            if(DirectoryScanDepth < 0)
            {
                throw new WalrusConfigurationException($"{nameof(DirectoryScanDepth)} must be >= 0");
            }

            // Be sure to validate against fully resolved paths
            foreach(var path in RepositoryRoots.Select(p => PathHelper.ResolvePath(p)))
            {
                if(string.IsNullOrEmpty(path))
                {
                    throw new WalrusConfigurationException($"{nameof(RepositoryRoots)} contains one or more empty paths. Perhaps an env var is misspelled?");
                }

                if(!Directory.Exists(path))
                {
                    throw new WalrusConfigurationException($"{nameof(RepositoryRoots)} contains {path} which does not exist");
                }
            }
        }
    }
}