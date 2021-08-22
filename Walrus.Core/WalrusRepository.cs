namespace Walrus.Core
{
    using LibGit2Sharp;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Walrus.Core.Internal;

    /// <summary>
    /// This wrapper provide safe access to some aspects of LibGit2Sharp.Repository
    /// that I found to be a little unstable.
    /// </summary>
    public class WalrusRepository
    {
        private readonly Repository _repository;

        /// <summary>
        /// Create a new WalrusRepository based on a git repository
        /// </summary>
        /// <param name="repository">Repository to wrap</param>
        internal WalrusRepository(Repository repository)
        {
            Ensure.IsNotNull(nameof(repository), repository);

            _repository = repository;
            RepositoryPath = Path.GetDirectoryName(repository.Info?.WorkingDirectory)!;
            RepositoryName = Path.GetFileName(RepositoryPath)!;
        }

        /// <summary>
        /// Name of folder containing Git repo
        /// </summary>
        public string RepositoryName { get; }

        /// <summary>
        /// Absolute path to Git repo
        /// </summary>
        public string RepositoryPath { get; }

        /// <summary>
        /// Returns list of all commits in this repo that satisfy the query
        /// </summary>
        /// <param name="query">Commit query</param>
        /// <returns>List of matching commits</returns>
        public IEnumerable<WalrusCommit> GetCommits(WalrusQuery query)
        {
            Ensure.IsNotNull(nameof(query), query);

            var iterators = new List<IEnumerator<Commit>>(32);

            if (query.AllBranches)
            {

                foreach (var branch in _repository.Branches)
                {
                    // Ignore remote branches because
                    // 1) they are slow
                    // 2) they might be shallow
                    if (branch.IsRemote)
                    {
                        continue;
                    }

                    iterators.Add(branch.Commits.GetEnumerator());
                }
            }
            else
            {
                iterators.Add(_repository.Commits.GetEnumerator());
            }

            foreach (var commitIter in iterators)
            {

                foreach (var commit in SafeGitCommitEnumeration(commitIter, query))
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
        private IEnumerable<WalrusCommit> SafeGitCommitEnumeration(IEnumerator<Commit> commitIter, WalrusQuery query)
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
                    WalrusLog.Logger.LogError(ex, "Error enumerating commits on {Path}", _repository.Info.WorkingDirectory);
                    break;
                }

                var commit = commitIter.Current;
                if (query.IsMatch(commit))
                {
                    yield return new WalrusCommit(this, commit);
                }

            } while (true);


            yield break;
        }
    }
}
