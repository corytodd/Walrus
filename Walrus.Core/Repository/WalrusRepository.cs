namespace Walrus.Core
{
    using System.Collections.Generic;
    using System.IO;
    using Walrus.Core.Internal;

    /// <summary>
    /// Git repository
    /// </summary>
    public class WalrusRepository
    {
        /// <summary>
        /// Create a new WalrusRepository based on a git repository
        /// </summary>
        /// <param name="repositoryPath">Path to git repository</param>
        /// <param name="commits">Repository commits</param>
        public WalrusRepository(string repositoryPath, IEnumerable<WalrusCommit> commits)
        {
            Ensure.IsValid(nameof(repositoryPath), !string.IsNullOrEmpty(repositoryPath));

            RepositoryPath = repositoryPath;
            RepositoryName = Path.GetFileName(RepositoryPath)!;
            Commits = commits;
        }

        /// <summary>
        /// Name of folder containing Git repo
        /// </summary>
        public string RepositoryName { get; }

        /// <summary>
        /// Absolute path to Git repo
        /// </summary>
        public string RepositoryPath { get; }
        
        /// <summary>
        /// All commits in this repository
        /// </summary>
        public IEnumerable<WalrusCommit> Commits { get; }
    }
}
