namespace Walrus.Core
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Walrus service provides access to system Git repositories
    /// </summary>
    public interface IWalrusService
    {
        /// <summary>
        /// Active configuration
        /// </summary>
        IWalrusConfig Config { get; }
        
        /// <summary>
        /// Executes the specified query
        /// </summary>
        /// <param name="query">Query to run</param>
        /// <returns>Resulting commit</returns>
        /// <exception cref="ArgumentOutOfRangeException">Raised if query grouping method is invalid</exception>
        IEnumerable<CommitGroup> ExecuteQuery(WalrusQuery query);

        /// <summary>
        /// Returns a list of all repositories that Walrus can see
        /// </summary>
        /// <param name="rootDirectory">Directory to search for Git repositories. If null or empty
        ///     <see cref="P:Config.RepositoryRoots"/> will be searched.</param>
        /// <param name="allBranches">True to include all branches in commit listing for each repo</param>
        /// <returns>List of repositories</returns>
        IEnumerable<WalrusRepository> GetAllRepositories(string? rootDirectory, bool allBranches);

    }
}