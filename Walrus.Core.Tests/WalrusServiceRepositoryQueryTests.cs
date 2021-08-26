namespace Walrus.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.Logging.Abstractions;
    using Xunit;

    /// <summary>
    ///     Test repository queries
    /// </summary>
    public class WalrusServiceRepositoryQueryTests
    {
        /// <summary>
        ///     Test that query returns expected values
        /// </summary>
        [Fact]
        public void QueryDefault()
        {
            // Setup
            var logger = new NullLogger<WalrusService>();
            var repoProvider = new MockRepoProvider();
            var config = WalrusConfig.Default;
            config.RepositoryRoots = new[] {"test"};
            var query = new WalrusQuery
            {
                After = ConstantDateTimes.LastWeek,
                Before = ConstantDateTimes.Tomorrow
            };

            // Execute
            var service = new WalrusService(logger, repoProvider, config);
            var commits = service.QueryCommits(query);

            // Assert
            Assert.NotEmpty(commits);
        }

        /// <summary>
        ///     Test that query returns expected values when grouped by date
        /// </summary>
        [Fact]
        public void QueryGroupByDate()
        {
            // Setup
            var logger = new NullLogger<WalrusService>();
            var repoProvider = new MockRepoProvider();
            var config = WalrusConfig.Default;
            config.RepositoryRoots = new[] {"test"};
            var query = new WalrusQuery
            {
                After = ConstantDateTimes.LastWeek,
                Before = ConstantDateTimes.Tomorrow,
                GroupBy = WalrusQuery.QueryGrouping.Date
            };

            // Execute
            var service = new WalrusService(logger, repoProvider, config);
            var commits = service.QueryCommits(query).ToList();

            // Assert
            Assert.NotEmpty(commits);
            Assert.All(commits, cg => Assert.True(cg.Key is DateTime));
            Assert.All(commits, cg => Assert.NotEmpty(cg.Data));
        }

        /// <summary>
        ///     Test that query returns expected values when grouped by repo
        /// </summary>
        [Fact]
        public void QueryGroupByRepo()
        {
            // Setup
            var logger = new NullLogger<WalrusService>();
            var repoProvider = new MockRepoProvider();
            var config = WalrusConfig.Default;
            config.RepositoryRoots = new[] {"test"};
            var query = new WalrusQuery
            {
                After = ConstantDateTimes.LastWeek,
                Before = ConstantDateTimes.Tomorrow,
                GroupBy = WalrusQuery.QueryGrouping.Repo
            };

            // Execute
            var service = new WalrusService(logger, repoProvider, config);
            var commits = service.QueryCommits(query).ToList();

            // Assert
            Assert.NotEmpty(commits);
            Assert.All(commits, cg => Assert.True(cg.Key is string));
            Assert.All(commits, cg => Assert.NotEmpty(cg.Data));
        }

        /// <summary>
        ///     Test that query returns expected values when grouped by author
        /// </summary>
        [Fact]
        public void QueryGroupByAuthor()
        {
            // Setup
            var logger = new NullLogger<WalrusService>();
            var repoProvider = new MockRepoProvider();
            var config = WalrusConfig.Default;
            config.RepositoryRoots = new[] {"test"};
            var query = new WalrusQuery
            {
                After = ConstantDateTimes.LastWeek,
                Before = ConstantDateTimes.Tomorrow,
                GroupBy = WalrusQuery.QueryGrouping.Author
            };

            // Execute
            var service = new WalrusService(logger, repoProvider, config);
            var commits = service.QueryCommits(query).ToList();

            // Assert
            Assert.NotEmpty(commits);
            Assert.All(commits, cg => Assert.True(cg.Key is string));
            Assert.All(commits, cg => Assert.NotEmpty(cg.Data));
        }

        /// <summary>
        ///     Test that an invalid grouping throws an exception
        /// </summary>
        [Fact]
        public void QueryGroupByInvalid()
        {
            // Setup
            var logger = new NullLogger<WalrusService>();
            var repoProvider = new MockRepoProvider();
            var config = WalrusConfig.Default;
            config.RepositoryRoots = new[] {"test"};
            var query = new WalrusQuery
            {
                After = ConstantDateTimes.LastWeek,
                Before = ConstantDateTimes.Tomorrow,
                GroupBy = (WalrusQuery.QueryGrouping) 42
            };

            // Execute
            var service = new WalrusService(logger, repoProvider, config);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => service.QueryCommits(query));
        }

        /// <summary>
        ///     Test that querying and filtering by repo name works
        /// </summary>
        [Fact]
        public void QueryFilterByRepo()
        {
            // Setup
            var logger = new NullLogger<WalrusService>();
            var repoProvider = new MockRepoProvider();
            var config = WalrusConfig.Default;
            config.RepositoryRoots = new[] {"test"};
            var query = new WalrusQuery
            {
                After = ConstantDateTimes.LastWeek,
                Before = ConstantDateTimes.Tomorrow,
                GroupBy = WalrusQuery.QueryGrouping.Repo,
                RepoName = "project_1"
            };

            // Execute
            var service = new WalrusService(logger, repoProvider, config);
            var commits = service.QueryCommits(query).ToList();

            // Assert
            Assert.NotEmpty(commits);
            Assert.All(commits, cg => Assert.Equal("project_1", cg.Key as string));
            Assert.All(commits, cg => Assert.NotEmpty(cg.Data));
            Assert.All(commits, cg => Assert.All(cg.Data, c => Assert.Equal("project_1", c.RepoName)));
        }

        /// <summary>
        ///     Test that querying and filtering by author works
        /// </summary>
        [Fact]
        public void QueryFilterByAuthor()
        {
            // Setup
            var logger = new NullLogger<WalrusService>();
            var repoProvider = new MockRepoProvider();
            var config = WalrusConfig.Default;
            config.RepositoryRoots = new[] {"test"};
            var query = new WalrusQuery
            {
                After = ConstantDateTimes.LastWeek,
                Before = ConstantDateTimes.Tomorrow,
                GroupBy = WalrusQuery.QueryGrouping.Author,
                AuthorEmail = "test@example.com"
            };

            // Execute
            var service = new WalrusService(logger, repoProvider, config);
            var commits = service.QueryCommits(query).ToList();

            // Assert
            Assert.NotEmpty(commits);
            Assert.All(commits, cg => Assert.Equal("test@example.com", cg.Key as string));
            Assert.All(commits, cg => Assert.NotEmpty(cg.Data));
            Assert.All(commits, cg => Assert.All(cg.Data, c => Assert.Equal("test@example.com", c.AuthorEmail)));
        }

        /// <summary>
        ///     Test that querying and filtering by author alias works
        /// </summary>
        [Fact]
        public void QueryFilterByAuthorAlias()
        {
            // Setup
            var logger = new NullLogger<WalrusService>();
            var repoProvider = new MockRepoProvider();
            var config = WalrusConfig.Default;
            config.RepositoryRoots = new[] {"test"};
            config.AuthorAliases = new Dictionary<string, IList<string>>
            {
                {"test", new[] {"test@examples.com", "test2@example.com"}}
            };
            var query = new WalrusQuery
            {
                AllBranches = true,
                After = ConstantDateTimes.LastWeek,
                Before = ConstantDateTimes.Tomorrow,
                CurrentDirectory = true,
                GroupBy = WalrusQuery.QueryGrouping.Author,
                AuthorAlias = "test"
            };

            // Execute
            var service = new WalrusService(logger, repoProvider, config);
            var commits = service.QueryCommits(query).ToList();

            // Assert
            Assert.NotEmpty(commits);
            Assert.All(commits, cg => Assert.True(config.AuthorAliases["test"].Contains(cg.Key as string)));
            Assert.All(commits, cg => Assert.NotEmpty(cg.Data));
        }

        /// <summary>
        ///     Test that query has a useful ToString
        /// </summary>
        [Fact]
        public void QueryToString()
        {
            // Setup
            var query = new WalrusQuery
            {
                AllBranches = true,
                After = ConstantDateTimes.LastWeek,
                Before = ConstantDateTimes.Tomorrow,
                CurrentDirectory = true,
                GroupBy = WalrusQuery.QueryGrouping.Author,
                AuthorAlias = "test"
            };

            // Assert
            Assert.Contains("After", query.ToString());
        }
    }
}