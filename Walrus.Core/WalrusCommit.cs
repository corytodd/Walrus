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

        public WalrusCommit(Commit commit)
        {
            _commit = commit;
        }

        /// <summary>
        /// Commit message text
        /// </summary>
        public string Message => _commit.Message;

        /// <summary>
        /// Date of commit
        /// </summary>
        public DateTime Timestamp => _commit.Author.When.DateTime;
    }
}
