namespace Walrus.Core.Tests
{
    using System;
    using Xunit;

    /// <summary>
    ///     PathHelper tests
    /// </summary>
    public class PathHelperTests
    {
        public PathHelperTests()
        {
            // setup some env vars
            Environment.SetEnvironmentVariable("HOME", "/home/user");
            Environment.SetEnvironmentVariable("FOO", "foo");
        }

        /// <summary>
        /// Assert that a path with no variables is not modified
        /// </summary>
        [Theory]
        [InlineData("/absolute/path", "/absolute/path")]
        [InlineData("relative", "relative")]
        [InlineData("relative/path/to/boot", "relative/path/to/boot")]
        [InlineData("relative/path/to/../boot", "relative/path/to/../boot")]
        public void NoVariablesTest(string input, string expect)
        {
            // Execute
            var resolved = PathHelper.ResolvePath(input);

            // Assert
            Assert.Equal(expect, resolved);
        }

        /// <summary>
        /// Assert that ~/ home resolution works
        /// </summary>
        [Theory]
        [InlineData("~/", "/home/user/")]
        public void HomeTildaTest(string input, string expect)
        {
            // Execute
            var resolved = PathHelper.ResolvePath(input);

            // Assert
            Assert.Equal(expect, resolved);
        }

        /// <summary>
        /// Assert that ~/ home and env var resolution works
        /// </summary>
        [Theory]
        [InlineData("~/$FOO", "/home/user/foo")]
        [InlineData("~/%FOO%", "/home/user/foo")]
        public void HomeVariablesTest(string input, string expect)
        {
            // Execute
            var resolved = PathHelper.ResolvePath(input);

            // Assert
            Assert.Equal(expect, resolved);
        }
    }
}