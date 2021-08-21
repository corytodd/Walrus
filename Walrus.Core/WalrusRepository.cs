namespace Walrus.Core
{
    using LibGit2Sharp;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// TODO: do we want to wrap this type or use the raw LibGit2 type?
    /// </summary>
    public class WalrusRepository
    {
        private readonly Repository _repository;

        internal WalrusRepository(Repository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Absolute path to Git repo
        /// </summary>
        public string? RepositoryPath => Path.GetDirectoryName(_repository?.Info?.WorkingDirectory);



        /// <summary>
        /// Most recent commit message
        /// </summary>
        public string? LastCommit => _repository.Head?.Tip?.Message;

        /// <summary>
        /// Returns list of all commits in this repo in specified time range
        /// </summary>
        /// <param name="start">Starting timestamp, inclusive</param>
        /// <param name="end">ending timestamp, exclusive</param>
        /// <returns>List of matching commits</returns>
        public IEnumerable<WalrusCommit> GetCommitsInRange(DateTime start, DateTime end)
        {
            var commitIter = _repository.Commits.GetEnumerator();
            foreach (var commit in SafeGitCommitEnumeration(commitIter, start, end))
            {
                yield return commit;
            }
        }


        /// <summary>
        /// Returns list of all commits in this repo on all branches in specified time range
        /// </summary>
        /// <param name="start">Starting timestamp, inclusive</param>
        /// <param name="end">ending timestamp, exclusive</param>
        /// <returns>List of matching commits</returns>
        public IEnumerable<WalrusCommit> GetCommitsInRangeAllBranches(DateTime start, DateTime end)
        {
            foreach (var branch in _repository.Branches)
            {
                if (branch.IsRemote)
                {
                    continue;
                }

                var commitIter = branch.Commits.GetEnumerator();
                foreach (var commit in SafeGitCommitEnumeration(commitIter, start, end))
                {
                    yield return commit;
                }
            }
        }

        /// <summary>
        /// LibGit2Sharp Commits iterator seems a bit crashy so we'll manually iterate
        /// </summary>
        /// <param name="commitIter">Commit iterator</param>
        /// <param name="start">Return commits on or after this date</param>
        /// <param name="end">Return commit before this date</param>
        /// <returns>Commits that satisfy filter</returns>
        private IEnumerable<WalrusCommit> SafeGitCommitEnumeration(IEnumerator<Commit> commitIter, DateTime start, DateTime end)
        {
            do
            {
                try
                {
                    // This may crash with an invalid object exception
                    if (!commitIter.MoveNext())
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    WalrusLog.Logger.LogError(ex, "Error enumerating commits on {Path}", RepositoryPath);
                    break;
                }

                var commit = commitIter.Current;
                if (commit.Author.When >= start && commit.Author.When < end)
                {
                    yield return new WalrusCommit(commit);
                }
            } while (true);


            yield break;
        }
    }
}
