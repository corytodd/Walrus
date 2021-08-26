namespace Walrus.Core.Tests
{
    using System;
    using System.Linq;
    using Microsoft.Extensions.Logging.Abstractions;
    using Xunit;

    /// <summary>
    /// Test repository queries
    /// </summary>
    public class WalrusServiceRepositoryQueryTests
    {
        /// <summary>
        /// Test that the default query returns expected values
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
            var commits = service.ExecuteQuery(query);

            // Assert
            Assert.NotEmpty(commits);
        }
        
        /// <summary>
        /// Test that the default query returns expected values when grouped by date
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
            var commits = service.ExecuteQuery(query);

            // Assert
            Assert.NotEmpty(commits);
            Assert.All(commits, cg => Assert.True(cg.Key is DateTime));
            Assert.All(commits, cg => Assert.NotEmpty(cg.Data));
        }
        
        /// <summary>
        /// Test that the default query returns expected values when grouped by repo
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
            var commits = service.ExecuteQuery(query).ToList();

            // Assert
            Assert.NotEmpty(commits);
            Assert.All(commits, cg => Assert.True(cg.Key is string));
            Assert.All(commits, cg => Assert.NotEmpty(cg.Data));
        }
        
        /// <summary>
        /// Test that the default query returns expected values when grouped by author
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
            var commits = service.ExecuteQuery(query).ToList();

            // Assert
            Assert.NotEmpty(commits);
            Assert.All(commits, cg => Assert.True(cg.Key is string));
            Assert.All(commits, cg => Assert.NotEmpty(cg.Data));
        }

        /// <summary>
        /// Test that an invalid grouping throws an exception
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
            Assert.Throws<ArgumentOutOfRangeException>(() => service.ExecuteQuery(query));
        }
    }
}