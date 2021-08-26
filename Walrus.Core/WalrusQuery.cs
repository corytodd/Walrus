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
        public DateTime After { get; init; }

        /// <summary>
        /// Include all commits before this date
        /// </summary>
        public DateTime Before { get; init; }

        /// <summary>
        /// Search all *local* branches
        /// </summary>
        public bool AllBranches { get; init; }
        
        /// <summary>
        /// Search for repository from current directory
        /// </summary>
        public bool CurrentDirectory { get; init; }

        /// <summary>
        /// Restrict results to commits from this author
        /// </summary>
        public string? AuthorEmail { get; init; }

        /// <summary>
        /// Author alias is an arbitrary string associated
        /// with one or more email addresses
        /// </summary>
        public string? AuthorAlias { get; init; }

        /// <summary>
        /// Restrict results to this repository
        /// </summary>
        public string? RepoName { get; init; }

        /// <summary>
        /// Group results using this rule
        /// </summary>
        public QueryGrouping GroupBy { get; init; }
        

        /// <inheritdoc />
        public override string ToString()
        {
            return $"After={After}, Before={Before}, AllBranches={AllBranches}, " +
                   $"CurrentDirectory={CurrentDirectory}, Repo={RepoName}, " +
                   $"Author.Email={AuthorEmail}, Author.Alias={AuthorAlias}, " +
                   $"Grouping={GroupBy}";
        }

        public enum QueryGrouping
        {
            /// <summary>
            /// Group commits by repo
            /// </summary>
            Repo,

            /// <summary>
            /// Group commits by date (specifically by day)
            /// </summary>
            Date,

            /// <summary>
            /// Group commits by author email or alias
            /// </summary>
            Author
        }
    }
}
