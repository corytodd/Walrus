namespace Walrus.Core.Tests
{
    using System.Collections.Generic;
    using Repository;

    /// <summary>
    ///     Repo provider for unit tests
    ///     Provides a constant set of repos and commits to test against
    /// </summary>
    public class MockRepoProvider : IRepositoryProvider
    {
        /// <inheritdoc />
        public IEnumerable<WalrusRepository> GetRepositories(string rootDirectory, int scanDepth, bool allBranches)
        {
            return new[]
            {
                new WalrusRepository("/home/user/code/project_1", new[]
                {
                    new WalrusCommit
                    {
                        Branch = "main",
                        Message = "[bugfix] fix issue #3133",
                        Sha = "0ee099f5a57d3149f49baadf61e6ace3eb635314",
                        Timestamp = ConstantDateTimes.LastWeek,
                        AuthorEmail = "test@example.com",
                        RepoName = "project_1",
                        RepoPath = "/home/user/code/project_1"
                    },
                    new WalrusCommit
                    {
                        Branch = "main",
                        Message = "[bugfix] revert fix for issue #3133",
                        Sha = "8ea142ee47e99a2641a0e64cbc75cc0e0d373115",
                        Timestamp = ConstantDateTimes.Yesterday,
                        AuthorEmail = "test@example.com",
                        RepoName = "project_1",
                        RepoPath = "/home/user/code/project_1"
                    },
                    new WalrusCommit
                    {
                        Branch = "main",
                        Message = "[bugfix] really fix issue #3133",
                        Sha = "2fe245c4374fcd72304333cead412926faa1dd2f",
                        Timestamp = ConstantDateTimes.Today,
                        AuthorEmail = "test@example.com",
                        RepoName = "project_1",
                        RepoPath = "/home/user/code/project_1"
                    }
                }),
                new WalrusRepository("/home/user/code/project_2", new[]
                {
                    new WalrusCommit
                    {
                        Branch = "main",
                        Message = "[bugfix] fix issue #2252",
                        Sha = "f47fb2990a0d9596e908a23a25e248b0b10a1f37",
                        Timestamp = ConstantDateTimes.LastWeek,
                        AuthorEmail = "test2@example.com",
                        RepoName = "project_2",
                        RepoPath = "/home/user/code/project_2"
                    },
                    new WalrusCommit
                    {
                        Branch = "main",
                        Message = "[bugfix] revert fix for issue #2252",
                        Sha = "515fec7ad07d728044271e7fa6a93a34217613b3",
                        Timestamp = ConstantDateTimes.Yesterday,
                        AuthorEmail = "test2@example.com",
                        RepoName = "project_2",
                        RepoPath = "/home/user/code/project_2"
                    },
                    new WalrusCommit
                    {
                        Branch = "main",
                        Message = "[bugfix] really fix issue #2252",
                        Sha = "0335f9a11ec0c5f83c0f412ed49e816f815d9089",
                        Timestamp = ConstantDateTimes.Today,
                        AuthorEmail = "test2@example.com",
                        RepoName = "project_2",
                        RepoPath = "/home/user/code/project_2"
                    }
                })
            };
        }
    }
}