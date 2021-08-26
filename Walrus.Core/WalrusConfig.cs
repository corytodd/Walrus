namespace Walrus.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// Walrus service configuration
    /// </summary>
    public sealed class WalrusConfig : IWalrusConfig
    {
        /// <inheritdoc />
        public int DirectoryScanDepth { get; set; }
        
        /// <inheritdoc />
        public IList<string>? RepositoryRoots { get; set; }

        /// <inheritdoc />
        public IDictionary<string, IList<string>>? AuthorAliases { get; set; }

        /// <summary>
        /// Generate a default configuration
        /// </summary>
        public static WalrusConfig Default => new()
        {
            DirectoryScanDepth = 3,
            RepositoryRoots = null,
            AuthorAliases = null
        };
    }
}
