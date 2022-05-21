namespace Walrus.Core
{
    using System.Linq;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    ///     Walrus service configuration
    /// </summary>
    public sealed class WalrusConfig : IWalrusConfig
    {
        private IList<string>? _repositoryRoots;

        /// <summary>
        ///     Generate a default configuration
        /// </summary>
        public static WalrusConfig Default => new()
        {
            DirectoryScanDepth = 3,
            RepositoryRoots = new List<string>(),
            AuthorAliases = null
        };

        /// <inheritdoc />
        public int DirectoryScanDepth { get; set; }

        /// <inheritdoc />
        public IList<string>? RepositoryRoots
        {
            get
            {   
                if(_repositoryRoots is null)
                {
                    return null;
                }

                // Return fully resolved paths (i.e. $HOME, %HOME%, ~/ expanded to the absolute path)
                return new List<string>(_repositoryRoots)
                    .Select(p => PathHelper.ResolvePath(p))
                    .ToList();
            }
            set => _repositoryRoots = value;
        }

        /// <inheritdoc />
        public IDictionary<string, IList<string>>? AuthorAliases { get; set; }

        /// <inheritdoc />
        public void ValidateOrThrow()
        {
            if(DirectoryScanDepth < 0)
            {
                throw new WalrusConfigurationException("DirectoryScanDepth must be >= 0");
            }

            if(RepositoryRoots is not null)
            {
                foreach(var path in RepositoryRoots)
                {
                    if(string.IsNullOrEmpty(path))
                    {
                        throw new WalrusConfigurationException("RepositoryRoots contains one or more empty paths. Perhaps an env var is misspelled?");
                    }            

                    if(!Directory.Exists(path))
                    {
                        throw new WalrusConfigurationException($"RepositoryRoots contains {path} which does not exist");
                    }
                }
            }
        }
    }
}