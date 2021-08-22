namespace Walrus.Core.Internal
{
    using LibGit2Sharp;
    using System;
    using System.Collections.Generic;
    using System.IO;

    internal static class Utilities
    {
        /// <summary>
        /// Returns a list of directories contained within the specified root directory 
        /// and its children up to the specified depth.
        /// </summary>
        /// <param name="root">Starting directory</param>
        /// <param name="depth">Scan depth. Set to zero for top-level only.</param>
        /// <returns></returns>
        public static IEnumerable<string> EnumerateDirectoriesToDepth(string root, int depth)
        {
            if (--depth > 0)
            {
                foreach (var directory in Directory.GetDirectories(root))
                {
                    // Do not dig deeper into the repo if we have found a root
                    if (Repository.IsValid(directory))
                    {
                        yield return directory;
                    }
                    else
                    {
                        foreach (var subDirectory in EnumerateDirectoriesToDepth(directory, depth))
                        {
                            yield return subDirectory;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Filter and return only valid Git repositories from the specified directory list
        /// </summary>
        /// <param name="directories">Directories to filter</param>
        /// <returns></returns>
        public static IEnumerable<WalrusRepository> GetValidRepositories(IEnumerable<string> directories)
        {
            foreach (var directory in directories)
            {

                if (!Repository.IsValid(directory))
                {
                    continue;
                }

                var repository = new Repository(directory);

                yield return new WalrusRepository(repository);
            }
        }
    }
}
