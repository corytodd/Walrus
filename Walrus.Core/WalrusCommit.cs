namespace Walrus.Core
{
    using LibGit2Sharp;
    using System;
    using Walrus.Core.Internal;

    /// <summary>
    /// Wraps commit information from Git
    /// </summary>
    public class WalrusCommit
    {
        /// <summary>
        /// Create a new commit
        /// </summary>
        /// <param name="repository">Repo that commit belongs to</param>
        /// <param name="commit">Commit</param>
        internal WalrusCommit(WalrusRepository repository, Commit commit)
        {
            Ensure.IsNotNull(nameof(repository), repository);
            Ensure.IsNotNull(nameof(commit), commit);

            RepoPath = repository.RepositoryPath;
            RepoName = repository.RepositoryName;
            Message = commit.MessageShort;
            Sha = commit.Sha;
            AuthorEmail = commit.Author.Email;
            Timestamp = commit.Author.When.DateTime;
        }
        /// <summary>
        /// Path to repository containing this commit5
        /// </summary>
        public string RepoPath { get; }

        /// <summary>
        /// Name of Git repo this commit belongs to
        /// </summary>
        public string RepoName { get; }

        /// <summary>
        /// Commit message text
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Commit hash
        /// </summary>
        public string Sha { get; }

        /// <summary>
        /// Email address of commit author
        /// </summary>
        public string AuthorEmail { get; }

        /// <summary>
        /// Date of commit
        /// </summary>
        public DateTime Timestamp { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[{RepoName}] {Timestamp} {AuthorEmail} {Sha} {Message}";
        }
    }
}
