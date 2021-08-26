namespace Walrus.Core.Repository
{
    using System;

    /// <summary>
    ///     Wraps commit information from Git
    /// </summary>
    public class WalrusCommit
    {
        /// <summary>
        ///     Branch this commit belongs to
        /// </summary>
        public string Branch { get; init; } = string.Empty;

        /// <summary>
        ///     Path to repository containing this commit
        /// </summary>
        public string RepoPath { get; init; } = string.Empty;

        /// <summary>
        ///     Name of Git repo this commit belongs to
        /// </summary>
        public string RepoName { get; init; } = string.Empty;

        /// <summary>
        ///     Commit message text
        /// </summary>
        public string Message { get; init; } = string.Empty;

        /// <summary>
        ///     Commit hash
        /// </summary>
        public string Sha { get; init; } = string.Empty;

        /// <summary>
        ///     Email address of commit author
        /// </summary>
        public string AuthorEmail { get; init; } = string.Empty;

        /// <summary>
        ///     Date of commit
        /// </summary>
        public DateTime Timestamp { get; init; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"[{RepoName}] {Timestamp} {AuthorEmail} {Sha} {Message}";
        }
    }
}