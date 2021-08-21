namespace Walrus.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// Warus service provides access to system Git repositories
    /// </summary>
    public interface IWalrusService
    {
        /// <summary>
        /// Active configuration
        /// </summary>
        WalrusConfig Config { get; }

        /// <summary>
        /// Returns a list of all repositories that Walrus can see
        /// </summary>
        /// <returns>List of repositories</returns>
        IEnumerable<WalrusRepository> GetRepositories();
    }
}