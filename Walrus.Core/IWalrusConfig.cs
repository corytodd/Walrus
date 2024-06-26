﻿// ReSharper disable UnusedMemberInSuper.Global

namespace Walrus.Core
{
    using System.Collections.Generic;

    /// <summary>
    ///     Configuration parameters for <see cref="IWalrusService" />
    /// </summary>
    public interface IWalrusConfig
    {
        /// <summary>
        ///     When scanning for repositories, limit search depth to this many
        ///     directories from each root.
        /// </summary>
        int DirectoryScanDepth { get; set; }

        /// <summary>
        ///     List of repository roots to scan
        /// </summary>
        IList<string> RepositoryRoots { get; set; }

        /// <summary>
        ///     List of author aliases
        ///     "some name" : { "email1@example.com", "email2@work.com" }
        /// </summary>
        IDictionary<string, IList<string>>? AuthorAliases { get; set; }

        /// <summary>
        ///     List of repository names to ignore
        ///     This is the full path of the repo to ignore
        /// </summary>
        IList<string> IgnoredRepos { get; set; }

        /// <summary>
        ///     Validates self or throws WalrusConfigurationException
        /// </summary>
        /// <exception cref="WalrusConfigurationException">Thrown if configuration is invalid</exception>
        public void ValidateOrThrow();
    }
}