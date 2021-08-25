namespace Walrus.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// Walrus service configuration
    /// </summary>
    public sealed class WalrusConfig
    {
        /// <summary>
        /// When scanning for repositories, limit search depth to this many 
        /// directories from each root.
        /// </summary>
        public int DirectoryScanDepth { get; set; }

        /// <summary>
        /// List of repository roots to scan
        /// </summary>
        public IList<string>? RepositoryRoots { get; set; }

        /// <summary>
        /// List of author aliases
        /// 
        /// "some name" : { "email1@example.com", "email2@work.com" }
        /// </summary>
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
