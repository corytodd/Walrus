namespace Walrus.Core
{
    using LibGit2Sharp;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Walrus.Core.Internal;

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
        /// Name of folder containing Git repo
        /// </summary>
        public string RepositoryName => Path.GetFileName(RepositoryPath);

        /// <summary>
        /// Absolute path to Git repo
        /// </summary>
        public string RepositoryPath => Path.GetDirectoryName(_repository?.Info?.WorkingDirectory)!;

        /// <summary>
        /// Most recent commit message
        /// </summary>
        public string? LastCommit => _repository.Head?.Tip?.Message;

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
                if (IsMatch(commit, query))
                {
                    yield return new WalrusCommit(this, commit);
                }
            } while (true);


            yield break;
        }

        /// <summary>
        /// Returns true if commit satisfies query
        /// </summary>
        /// <param name="commit">Commit to test</param>
        /// <param name="query">Query parameters</param>
        /// <returns>True if commit satisfies query</returns>
        private bool IsMatch(Commit commit, WalrusQuery query)
        {
            var isMatch = true;

            if (!string.IsNullOrEmpty(query.AuthorEmail))
            {
                isMatch = commit.Author.Email == query.AuthorEmail;
            }            

            return isMatch && commit.Author.When >= query.After && commit.Author.When < query.Before;
        }
    }
}
