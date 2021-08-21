﻿using System.Collections.Generic;

namespace Walrus.Core
{
    /// <summary>
    /// Walrus service configuration
    /// </summary>
    public sealed class WalrusConfig
    {
        /// <summary>
        /// When scanning for repositories, limit search depth to this many 
        /// directories from each root.
        /// </summary>
        public int DirectoryScanDepth { get; set; }

        /// <summary>
        /// List of repository roots to scan
        /// </summary>
        public IList<string> RepositoryRoots { get; set;  }
    }
}
