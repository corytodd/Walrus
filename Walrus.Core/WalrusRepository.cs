using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;

namespace Walrus.Core
{
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
        public string RepositoryPath => Path.GetDirectoryName(_repository.Info.WorkingDirectory);

        /// <summary>
        /// Most recent commit message
        /// </summary>
        public string LastCommit => _repository.Head?.Tip?.Message;

        /// <summary>
        /// Returns list of all commits in this repo in specified time range
        /// </summary>
        /// <param name="start">Starting timestamp, inclusive</param>
        /// <param name="end">ending timestamp, exclusive</param>
        /// <returns>List of matching commits</returns>
        public IEnumerable<WalrusCommit> GetCommitsInRange(DateTime start, DateTime end)
        {
            foreach(var commit in _repository.Commits)
            {
                if(commit.Author.When >= start && commit.Author.When < end)
                {
                    yield return new WalrusCommit(commit);
                }
            }
        }
    }
}
