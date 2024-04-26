namespace Walrus.Core.Repository
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Internal;
    using LibGit2Sharp;
    using Microsoft.Extensions.Logging;

    /// <summary>
    ///     Provides access to Git repositories on the file system
    /// </summary>
    public class RepositoryProvider : IRepositoryProvider
    {
        /// <inheritdoc />
        public IEnumerable<WalrusRepository> GetRepositories(string rootDirectory, int scanDepth, bool allBranches, Predicate<string>? excludeFilter =null)
        {
            Ensure.IsValid(nameof(rootDirectory), !string.IsNullOrEmpty(rootDirectory));
            Ensure.IsValid(nameof(scanDepth), scanDepth >= 0);

            // Accept everything by default
            excludeFilter ??= (string s) => false;

            var directories = Utilities.EnumerateGitDirectoriesToDepth(rootDirectory, scanDepth);

            foreach (var directory in directories)
            {
                Ensure.IsValid(nameof(directory), Repository.IsValid(directory), "There is a bug in EnumerateGitDirectoriesToDepth");
                var repo = new Repository(directory);

                // Bare repos do not set working directory, fallback to path
                var repoPath = repo.Info.WorkingDirectory ?? repo.Info.Path;

                // If neither the path or the working directory are set
                Ensure.IsValid(nameof(repoPath), !string.IsNullOrEmpty(repoPath), "Expected a valid repo path to be set. This is a bug.");

                if (excludeFilter(repoPath))
                {
                    continue;
                }

                var commits = GetCommits(repo, allBranches);

                var repository = new WalrusRepository(repoPath, commits);
                yield return repository;
            }
        }

        /// <summary>
        ///     Returns list of all commits in this repo
        /// </summary>
        /// <param name="repo">Repo to get commits from</param>
        /// <param name="allBranches">True to include commits from all local branches</param>
        /// <returns>List of commits in this repo</returns>
        private static IEnumerable<WalrusCommit> GetCommits(IRepository repo, bool allBranches)
        {
            Ensure.IsNotNull(nameof(repo), repo);

            // List of tuple(branch name, commit iterator), assume no more than 32 branches
            var iterators = new List<(string, IEnumerator<Commit>)>(32);

            if (allBranches)
            {
                // ReSharper disable once LoopCanBeConvertedToQuery - this is more legible
                foreach (var branch in repo.Branches)
                {
                    // Ignore remote branches because
                    // 1) they are slow
                    // 2) they might be shallow
                    if (branch.IsRemote)
                    {
                        continue;
                    }

                    iterators.Add((branch.FriendlyName, branch.Commits.GetEnumerator()));
                }
            }
            else
            {
                iterators.Add((repo.Head.FriendlyName, repo.Commits.GetEnumerator()));
            }

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator - this is debuggable
            foreach (var commitIter in iterators)
            {
                foreach (var commit in SafeGitCommitEnumeration(repo, commitIter))
                {
                    yield return commit;
                }
            }
        }


        /// <summary>
        ///     LibGit2Sharp Commits iterator seems a bit crashy so we'll manually iterate
        /// </summary>
        /// <param name="repository">Repository to get commits from</param>
        /// <param name="branchIterator">List of branches and their respective commit iterators</param>
        /// <returns>List of commits in this repo</returns>
        private static IEnumerable<WalrusCommit> SafeGitCommitEnumeration(IRepository repository,
            (string, IEnumerator<Commit>) branchIterator)
        {
            do
            {
                var (branchName, iter) = branchIterator;

                try
                {
                    // This may crash with an invalid object exception
                    if (!iter.MoveNext())
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    WalrusLog.Logger.LogError(ex, "Error enumerating commits on {Path}",
                        repository.Info.WorkingDirectory);
                    break;
                }

                var repoPath = repository.Info.WorkingDirectory!;
                var repoName = Path.GetFileName(Path.GetDirectoryName(repoPath))!;

                var commit = iter.Current;
                yield return new WalrusCommit
                {
                    Branch = branchName,
                    Message = commit.MessageShort,
                    Sha = commit.Sha,
                    Timestamp = commit.Author.When.DateTime,
                    AuthorEmail = commit.Author.Email,
                    RepoName = repoName,
                    RepoPath = repoPath
                };
            } while (true);
        }
    }
}