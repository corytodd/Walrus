namespace Walrus.Core.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Xunit;

    /// <summary>
    ///     PathHelper tests
    /// </summary>
    public class PathHelperTests
    {
        /// <summary>
        ///     Provides platform dependent paths for testing
        /// </summary>
        public class PathHelperTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                if (OperatingSystem.IsWindows())
                {
                    yield return new object[] { @"C:\absolute\path", @"C:\absolute\path" };
                    yield return new object[] { "relative", "relative" };
                    yield return new object[] { @"relative\path\to\boot", @"relative\path\to\boot" };
                    yield return new object[] { @"relative\path\to\..\boot", @"relative\path\to\..\boot" };
                    yield return new object[] { @"~/", @"C:\Users\user\" };
                    yield return new object[] { @"~/%FOO%", @"C:\Users\user\foo" };
                }
                else
                {
                    yield return new object[] { "/absolute/path", "/absolute/path" };
                    yield return new object[] { "relative", "relative" };
                    yield return new object[] { "relative/path/to/boot", "relative/path/to/boot" };
                    yield return new object[] { "relative/path/to/../boot", "relative/path/to/../boot" };
                    yield return new object[] { "~/", "/home/user/" };
                    yield return new object[] { "~/$FOO", "/home/user/foo" };
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public PathHelperTests()
        {
            // setup some env vars
            if (OperatingSystem.IsWindows())
            {
                Environment.SetEnvironmentVariable("HOME", @"C:\Users\user");
            }
            else
            {
                Environment.SetEnvironmentVariable("HOME", "/home/user");
            }

            Environment.SetEnvironmentVariable("FOO", "foo");
        }

        /// <summary>
        /// Assert that a path with no variables is not modified
        /// </summary>
        [Theory]
        [ClassData(typeof(PathHelperTestData))]
        public void NoVariablesTest(string input, string expect)
        {
            // Execute
            var resolved = PathHelper.ResolvePath(input);

            // Assert
            Assert.Equal(expect, resolved);
        }
    }
}