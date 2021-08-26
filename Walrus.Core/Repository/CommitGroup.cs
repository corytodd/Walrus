namespace Walrus.Core.Repository
{
    using System.Collections.Generic;

    /// <summary>
    ///     Query grouping wrapper allows for arbitrary key grouping
    /// </summary>
    public class CommitGroup
    {
        /// <summary>
        ///     Create a new commit group
        /// </summary>
        /// <param name="key">Group key</param>
        /// <param name="data">Commits</param>
        public CommitGroup(object key, IEnumerable<WalrusCommit> data)
        {
            Key = key;
            Data = data;
        }

        /// <summary>
        ///     Grouping key
        /// </summary>
        public object Key { get; }

        /// <summary>
        ///     Commits for this group
        /// </summary>
        public IEnumerable<WalrusCommit> Data { get; }
    }
}