namespace Walrus.Core.Tests
{
    using System.Linq;
    using System.Collections.Generic;
    using Xunit;

    /// <summary>
    ///     WalrusConfig tests
    /// </summary>
    public class WalrusConfigTests
    {
        /// <summary>
        /// The default configuration should be valid
        /// </summary>
        [Fact]
        public void ValidConfigurationTest()
        {
            // Setup
            var config = WalrusConfig.Default;

            // Execute
            config.ValidateOrThrow();
        }

        /// <summary>
        /// A configuration file with negative scan depth is invalid
        /// </summary>
        [Fact]
        public void NegativeScanDepthTest()
        {
            // Setup
            var config = WalrusConfig.Default;
            config.DirectoryScanDepth = -1;

            // Execute
            Assert.Throws<WalrusConfigurationException>(() => config.ValidateOrThrow());
        }

        /// <summary>
        /// A configuration file with an empty path is invalid
        /// </summary>
        [Fact]
        public void EmptyPathRootTest()
        {
            // Setup
            var config = WalrusConfig.Default;
            config.RepositoryRoots.Add(string.Empty);

            // Execute
            Assert.Throws<WalrusConfigurationException>(() => config.ValidateOrThrow());
        }
    }
}