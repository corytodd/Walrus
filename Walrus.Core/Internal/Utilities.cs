namespace Walrus.Core.Internal
{
    using System.Collections.Generic;
    using System.IO;
    using LibGit2Sharp;

    internal static class Utilities
    {
        /// <summary>
        ///     Returns a list of Git directories contained within the specified root directory
        ///     and its children up to the specified depth.
        /// </summary>
        /// <param name="root">Starting directory</param>
        /// <param name="depth">Scan depth. Set to zero for top-level only.</param>
        /// <returns></returns>
        public static IEnumerable<string> EnumerateGitDirectoriesToDepth(string root, int depth)
        {
            if (--depth <= 0)
            {
                yield break;
            }

            // If we need compat < net5.0 replace this with a try/catch around yet another generator
            var options = new EnumerationOptions {IgnoreInaccessible = false};
            foreach (var directory in Directory.GetDirectories(root, "*", options))
            {
                // Do not dig deeper into the repo if we have found a root
                if (Repository.IsValid(directory))
                {
                    yield return directory;
                }
                else
                {
                    foreach (var subDirectory in EnumerateGitDirectoriesToDepth(directory, depth))
                    {
                        yield return subDirectory;
                    }
                }
            }
        }
    }
}