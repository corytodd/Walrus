namespace Walrus.Core
{
    using System;
    using System.Collections.Generic;
    using Repository;

    /// <summary>
    ///     Walrus service provides access to system Git repositories
    /// </summary>
    public interface IWalrusService
    {
        /// <summary>
        ///     Active configuration
        /// </summary>
        IWalrusConfig Config { get; }

        /// <summary>
        ///     Executes the specified query
        /// </summary>
        /// <param name="query">Query to run</param>
        /// <returns>Resulting commit</returns>
        /// <exception cref="ArgumentOutOfRangeException">Raised if query grouping method is invalid</exception>
        IEnumerable<CommitGroup> QueryCommits(WalrusQuery query);

        /// <summary>
        ///     Returns a list of all repositories that Walrus can see
        /// </summary>
        /// <param name="query">Repos</param>
        /// <returns>List of repositories</returns>
        IEnumerable<WalrusRepository> QueryRepositories(WalrusQuery? query = null);
    }
}