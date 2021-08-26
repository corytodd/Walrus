namespace Walrus.Core.Tests
{
    using System;
    using System.Linq;
    using Microsoft.Extensions.Logging.Abstractions;
    using Xunit;

    public class WalrusServiceRepositoryTests
    {
        /// <summary>
        /// Assert that when no search root is specified we get no results
        /// </summary>
        [Fact]
        public void GetRepositoriesNoRoots()
        {
            // Setup
            var logger = new NullLogger<WalrusService>();
            var repoProvider = new MockRepoProvider();
            var config = WalrusConfig.Default;

            // Execute
            var service = new WalrusService(logger, repoProvider, config);
            var repos = service.QueryRepositories().ToList();

            // Assert
            Assert.Empty(repos);
        }

        /// <summary>
        /// Assert that all fetched repositories have their properties set
        /// </summary>
        [Fact]
        public void GetRepositories()
        {
            // Setup
            var logger = new NullLogger<WalrusService>();
            var repoProvider = new MockRepoProvider();
            var config = WalrusConfig.Default;
            var query = new WalrusQuery {CurrentDirectory = true};

            // Execute
            var service = new WalrusService(logger, repoProvider, config);
            var repos = service.QueryRepositories(query).ToList();

            // Assert
            Assert.NotEmpty(repos);
            Assert.All(repos, r => Assert.True(!string.IsNullOrEmpty(r.RepositoryName)));
            Assert.All(repos, r => Assert.True(!string.IsNullOrEmpty(r.RepositoryPath)));
            Assert.All(repos, r => Assert.True(r.Commits.Any()));
        }

        /// <summary>
        /// Assert that all fetched repositories have their properties set
        /// </summary>
        [Fact]
        public void GetCommitsRepositories()
        {
            // Setup
            var logger = new NullLogger<WalrusService>();
            var repoProvider = new MockRepoProvider();
            var config = WalrusConfig.Default;
            var query = new WalrusQuery {CurrentDirectory = true};

            // Execute
            var service = new WalrusService(logger, repoProvider, config);
            var repos = service.QueryRepositories(query).ToList();
            var commits = repos.SelectMany(r => r.Commits).ToList();

            // Assert
            Assert.NotEmpty(commits);
            Assert.All(commits, c => Assert.True(!string.IsNullOrEmpty(c.Branch)));
            Assert.All(commits, c => Assert.True(!string.IsNullOrEmpty(c.RepoPath)));
            Assert.All(commits, c => Assert.True(!string.IsNullOrEmpty(c.RepoName)));
            Assert.All(commits, c => Assert.True(!string.IsNullOrEmpty(c.Message)));
            Assert.All(commits, c => Assert.True(!string.IsNullOrEmpty(c.Sha)));
            Assert.All(commits, c => Assert.True(!string.IsNullOrEmpty(c.AuthorEmail)));
            Assert.All(commits, c => Assert.NotEqual(DateTime.MinValue, c.Timestamp));
            Assert.All(commits, c => Assert.Contains(c.RepoName, c.ToString()));
        }
    }
}