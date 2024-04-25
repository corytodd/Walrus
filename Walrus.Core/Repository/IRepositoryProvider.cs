namespace Walrus.Core.Repository
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     Provides access to Git repositories
    /// </summary>
    public interface IRepositoryProvider
    {
        /// <summary>
        ///     Returns a list of all repositories under this directory up to the specified depth
        /// </summary>
        /// <param name="rootDirectory">Directory to search for Git repositories</param>
        /// <param name="scanDepth">Recursively scan to this depth. Must be greater than or equal to zero.</param>
        /// <param name="allBranches">If true, include all branches in commit collection for each repository.</param>
        /// <param name="excludeFilter">Optional filter returns false for repo paths that should be excluded.</param>
        /// <returns>List of repositories</returns>
        /// <exception cref="ArgumentException"><paramref name="scanDepth" /> is less than 0</exception>
        IEnumerable<WalrusRepository> GetRepositories(string rootDirectory, int scanDepth, bool allBranches, Predicate<string>? excludeFilter =null);
    }
}