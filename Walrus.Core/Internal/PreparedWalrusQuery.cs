namespace Walrus.Core.Internal
{
    using System;
    using System.Collections.Generic;
    using Repository;

    /// <summary>
    ///     Prepared and cached Walrus query
    /// </summary>
    internal class PreparedWalrusQuery
    {
        /// <summary>
        ///     List of email addresses to filter commits on
        /// </summary>
        private readonly HashSet<string>? _aliasEmails;

        private readonly WalrusQuery _query;

        /// <summary>
        ///     Prepare a new query
        /// </summary>
        /// <param name="query">Query parameters</param>
        /// <param name="config">Service configuration</param>
        public PreparedWalrusQuery(WalrusQuery query, IWalrusConfig config)
        {
            Ensure.IsNotNull(nameof(query), query);
            Ensure.IsNotNull(nameof(config), config);

            _query = query;

            if (HasMatchingAliasMap(config))
            {
                _aliasEmails = new HashSet<string>(config.AuthorAliases![_query.AuthorAlias!]);
            }

            // If current directory is specified it will override the configured search roots
            SearchPaths = query.CurrentDirectory ? new[] {Environment.CurrentDirectory} : config.RepositoryRoots;
        }

        /// <summary>
        ///     Returns true if all local branches should be returned
        /// </summary>
        public bool AllBranches => _query.AllBranches;

        /// <summary>
        ///     List of paths to search for repositories
        /// </summary>
        public IEnumerable<string>? SearchPaths { get; }

        /// <summary>
        ///     Returns true if commit satisfies this query
        /// </summary>
        /// <param name="commit">Commit to test</param>
        /// <returns>True if commit satisfies this query</returns>
        public bool IsMatch(WalrusCommit commit)
        {
            var isMatch = true;

            // Direct email (non-alias) takes precedence over aliases
            if (!string.IsNullOrEmpty(_query.AuthorEmail))
            {
                isMatch = commit.AuthorEmail == _query.AuthorEmail;
            }
            else if (_aliasEmails is not null)
            {
                isMatch = _aliasEmails.Contains(commit.AuthorEmail);
            }

            return isMatch && commit.Timestamp >= _query.After && commit.Timestamp < _query.Before;
        }

        /// <summary>
        ///     Returns true if repository satisfies this query
        /// </summary>
        /// <param name="repository">Repository to test</param>
        /// <returns>True if repository satisfies this query</returns>
        public bool IsMatch(WalrusRepository repository)
        {
            return string.IsNullOrEmpty(_query.RepoName) ||
                   repository.RepositoryName.Equals(_query.RepoName, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        ///     Returns true if this query matches an alias with an email mapping
        /// </summary>
        /// <param name="config">Configuration to check</param>
        /// <returns>True if the query should check for email aliases</returns>
        private bool HasMatchingAliasMap(IWalrusConfig config)
        {
            return !string.IsNullOrEmpty(_query.AuthorAlias) &&
                   config.AuthorAliases is not null &&
                   config.AuthorAliases.ContainsKey(_query.AuthorAlias);
        }
    }
}