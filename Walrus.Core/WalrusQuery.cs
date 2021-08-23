namespace Walrus.Core
{
    using LibGit2Sharp;
    using System;
    using System.Collections.Generic;
    using Walrus.Core.Internal;

    /// <summary>
    /// Commit query parameters
    /// </summary>
    public class WalrusQuery
    {
        /// <summary>
        /// List of email addresses to filter commits on
        /// </summary>
        private HashSet<string>? _aliasEmails;

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
        /// Author alias is an arbitrary string associated
        /// with one or more email addresses
        /// </summary>
        public string? AuthorAlias { get; set; }

        /// <summary>
        /// Restrict results to this repository
        /// </summary>
        public string? RepoName { get; set; }

        /// <summary>
        /// Group results using this rule
        /// </summary>
        public QueryGrouping GroupBy { get; set; }

        /// <summary>
        /// Assign service configuration to this query which will augment
        /// certain query parameters. This includes:
        /// - the user alias feature.
        /// </summary>
        /// <param name="config">Service configuration</param>
        public void AddConfiguration(WalrusConfig config)
        {
            Ensure.IsNotNull(nameof(config), config);

            if (HasMatchingAliasMap(config))
            {
                _aliasEmails = new HashSet<string>(config.AuthorAliases![AuthorAlias!]);
            }
        }

        /// <summary>
        /// Returns true if commit satisfies this query
        /// </summary>
        /// <param name="commit">Commit to test</param>
        /// <returns>True if commit satisfies this query</returns>
        public bool IsMatch(Commit commit)
        {
            var isMatch = true;

            // Direct email (non-alias) takes precedence over aliases
            if (!string.IsNullOrEmpty(AuthorEmail))
            {
                isMatch = commit.Author.Email == AuthorEmail;
            }
            else if (_aliasEmails is not null)
            {
                isMatch = _aliasEmails.Contains(commit.Author.Email);
            }

            return isMatch && commit.Author.When >= After && commit.Author.When < Before;
        }

        /// <summary>
        /// Returns true if this query matches an alias with an email mapping
        /// </summary>
        /// <param name="config">Configuration to check</param>
        /// <returns>True if the query should check for email aliases</returns>
        private bool HasMatchingAliasMap(WalrusConfig config)
        {
            return !string.IsNullOrEmpty(AuthorAlias) &&
                config.AuthorAliases is not null &&
                config.AuthorAliases.ContainsKey(AuthorAlias);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"After={After}, Before={Before}, AllBranches={AllBranches}, Author.Email={AuthorEmail}, Author.Alias={AuthorAlias}, Grouping={GroupBy}";
        }

        public enum QueryGrouping
        {
            /// <summary>
            /// Group commits by repo
            /// </summary>
            Repo,

            /// <summary>
            /// Group commits by date (specifcally by day)
            /// </summary>
            Date,

            /// <summary>
            /// Group commits by author email or alias
            /// </summary>
            Author
        }
    }
}
