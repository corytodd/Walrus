namespace Walrus.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Path helper utilities
    /// </summary>
    /// <remarks>Nested variables are not supported</remarks>
    public static class PathHelper
    {
        /// <summary>
        /// Automatically resolve all environmental variables and
        /// the home tilda (~/) in path.
        /// </summary>
        /// <param name="path">Path to resolve</param>
        /// <return>Absolute resolved path</return>
        public static string ResolvePath(string path)
        {
            ArgumentNullException.ThrowIfNull(path);

            if (path.StartsWith("~/"))
            {
                var home = Environment.GetEnvironmentVariable("HOME");
                path = path.Replace("~/", $"{home}{Path.DirectorySeparatorChar}");
            }

            path = ResolveAllEnvironmentVariables(path);

            return path;

        }

        /// <summary>
        /// Returns true if <paramref name="pathList"/> contains <paramref name="repositoryPath"/>.
        /// </summary>
        /// <param name="ignoredRepos">List of paths to test against</param>
        /// <param name="repositoryPath"></param>
        /// <returns></returns>
        internal static bool ContainsPath(IList<string> pathList, string repositoryPath)
        {
            var found = false;
            var normalizedPath = Path.GetFullPath(ResolvePath(repositoryPath));
            foreach(var ignoredRepo in pathList)
            {
                var normalizedIgnoredRepo = Path.GetFullPath(ResolvePath(ignoredRepo));
                if (normalizedIgnoredRepo == normalizedPath)
                {
                    found = true;
                    break;
                }
            }
            return found;
        }

        /// <summary>
        /// Split path into components and resolve them if they're environmental
        /// variables. It turns out Environment.ExpandEnvironmentVariables does
        /// not support the $VAR syntax, only %VAR% syntax.
        /// </summary>
        private static string ResolveAllEnvironmentVariables(string path)
        {
            var resolved = new List<string>(16);
            foreach (var part in path.Split(Path.DirectorySeparatorChar))
            {
                string expanded = part;

                if (part.StartsWith('$'))
                {
                    var varName = part.Substring(1);
                    expanded = Environment.GetEnvironmentVariable(varName) ?? string.Empty;
                }
                else if (part.StartsWith('%'))
                {
                    expanded = Environment.ExpandEnvironmentVariables(part);
                }

                resolved.Add(expanded);
            }

            return string.Join(Path.DirectorySeparatorChar, resolved);
        }
    }
}