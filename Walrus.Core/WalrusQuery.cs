namespace Walrus.Core
{
    using System;

    /// <summary>
    /// Commit query parameters
    /// </summary>
    public class WalrusQuery
    {
        /// <summary>
        /// Include all commits on or after this date
        /// </summary>
        public DateTime After { get; set; }

        /// <summary>
        /// Include all commits before this date
        /// </summary>
        public DateTime Before { get; set; }

        /// <summary>
        /// Search all *local* branches
        /// </summary>
        public bool AllBranches { get; set; }

        /// <summary>
        /// Restrict results to commits from this author
        /// </summary>
        public string? AuthorEmail { get; set; }

        /// <summary>
        /// Restrict results to this repository
        /// </summary>
        public string? RepoName { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"After={After}, Before={Before}, AllBranches={AllBranches}, Author.Email={AuthorEmail}";
        }
    }
}
