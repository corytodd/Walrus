namespace Walrus.Core.Tests
{
    using System.Collections.Generic;

    /// <summary>
    /// Repo provider for unit tests
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
                })
            };
        }
    }
}