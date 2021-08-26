namespace Walrus.Core.Tests
{
    using System;

    /// <summary>
    ///     Provides fixed datetime values for test consistency
    /// </summary>
    public static class ConstantDateTimes
    {
        public static readonly DateTime Today = new(2021, 8, 25, 19, 35, 00);

        public static readonly DateTime Tomorrow = Today.AddDays(1);

        public static readonly DateTime Yesterday = Today.AddDays(-1);

        public static readonly DateTime LastWeek = Today.AddDays(-7);
    }
}