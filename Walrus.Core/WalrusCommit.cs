namespace Walrus.Core
{
    using LibGit2Sharp;
    using System;

    /// <summary>
    /// TODO: do we want to wrap this type or use the raw LibGit2 type?
    /// </summary>
    public class WalrusCommit
    {
        private readonly Commit _commit;

        public WalrusCommit(string repoName, Commit commit)
        {
            RepoName = repoName;
            _commit = commit;
        }

        /// <summary>
        /// Name of Git repo this commit belongs to
        /// </summary>
        public string RepoName { get; }

        /// <summary>
        /// Commit message text
        /// </summary>
        public string Message => _commit.MessageShort;

        /// <summary>
        /// Commit hash
        /// </summary>
        public string Sha => _commit.Sha;

        /// <summary>
        /// Date of commit
        /// </summary>
        public DateTime Timestamp => _commit.Author.When.DateTime;

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[{RepoName}] {Timestamp} {_commit.Author.Email} {Sha} {Message}";
        }
    }
}
