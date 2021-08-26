namespace Walrus.Core.Tests
{
    using System;

    /// <summary>
    /// Provides fixed datetime values for test consistency
    /// </summary>
    public static class ConstantDateTimes
    {
        public static DateTime Today = new DateTime(2021, 8, 25, 19, 35, 00);

        public static DateTime Tomorrow = Today.AddDays(1);
        
        public static DateTime Yesterday = Today.AddDays(-1);
        
        public static DateTime LastWeek = Today.AddDays(-7);
    }
}