namespace Walrus.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging.Abstractions;
    using Xunit;

    public class WalrusServiceTests
    {
        /// <summary>
        /// Assert that constructor parameters are validated
        /// </summary>
        [Fact]
        public void LoggerRequired()
        {
            // Setup
            NullLogger<WalrusService> logger = null;
            var config = WalrusConfig.Default;

            // Execute
            // Assert
            Assert.Throws<ArgumentNullException>(() => new WalrusService(logger, config));
        }
        
        /// <summary>
        /// Assert that constructor parameters are validated
        /// </summary>
        [Fact]
        public void ConfigRequired()
        {
            // Setup
            var logger = new NullLogger<WalrusService>();
            WalrusConfig config = null;

            // Execute
            // Assert
            Assert.Throws<ArgumentNullException>(() => new WalrusService(logger, config));
        }
        
                
        /// <summary>
        /// Assert that query parameters are validated
        /// </summary>
        [Fact]
        public void QueryRequired()
        {
            // Setup
            var logger = new NullLogger<WalrusService>();
            var config = WalrusConfig.Default;

            // Execute
            var service = new WalrusService(logger, config);
            
            // Assert
            Assert.Throws<ArgumentNullException>(() => service.ExecuteQuery(null));
        }

                
        /// <summary>
        /// Assert that configuration is captured
        /// </summary>
        [Fact]
        public void ConfigStored()
        {
            // Setup
            var logger = new NullLogger<WalrusService>();
            var config = WalrusConfig.Default;
            config.AuthorAliases = new Dictionary<string, IList<string>> {{"test", new[] {"hello@example.com"}}};
            config.RepositoryRoots = new[] {"/home/user/code", "C:\\code\\foo"};

            // Execute
            var service = new WalrusService(logger, config);
            
            // Assert
            Assert.Equal(config.DirectoryScanDepth, service.Config.DirectoryScanDepth);
            Assert.Equal(config.RepositoryRoots, service.Config.RepositoryRoots);
            Assert.Equal(config.AuthorAliases, service.Config.AuthorAliases);
        }
    }
}
