namespace Walrus.Core
{
    using LibGit2Sharp;
    using System;
    using System.IO;
    using Walrus.Core.Internal;

    /// <summary>
    /// TODO: do we want to wrap this type or use the raw LibGit2 type?
    /// </summary>
    public class WalrusCommit
    {
        private readonly WalrusRepository _repository;
        private readonly Commit _commit;

        public WalrusCommit(WalrusRepository repository, Commit commit)
        {
            Ensure.IsNotNull(nameof(repository), repository);
            Ensure.IsNotNull(nameof(commit), commit);

            _repository = repository;
            _commit = commit;
        }
        /// <summary>
        /// Path to repository containing this commit5
        /// </summary>
        public string RepoPath => _repository.RepositoryPath!;

        /// <summary>
        /// Name of Git repo this commit belongs to
        /// </summary>
        public string RepoName => _repository.RepositoryName!;

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
