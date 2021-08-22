namespace Walrus.CLI
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Linq;
    using System.Reflection;
    using Walrus.Core;

    internal static class ServiceExtensions
    {
        private static readonly Lazy<Assembly> ThisAssembly = new(() => typeof(Program).Assembly);

        /// <summary>
        /// Discover and attach all Commands in this assembly
        /// Note: Taken from https://endjin.com/blog/2020/09/simple-pattern-for-using-system-commandline-with-dependency-injection
        /// </summary>
        /// <param name="services">Service context</param>
        /// <returns>Service collection</returns>
        public static IServiceCollection AddCliCommands(this IServiceCollection services)
        {
            // All command should dervice from BaseCommand but we'll accept the base interface as well
            var commandType = typeof(IComposableCommand);
            var commands = ThisAssembly.Value
                .GetTypes()
                .Where(x => !x.IsAbstract && commandType.IsAssignableFrom(x));

            foreach (var command in commands)
            {
                services.AddSingleton(commandType, command);
            }

            return services;
        }

        /// <summary>
        /// Add fluent Walrus logging support
        /// Note: Inspired by https://github.com/NLog/NLog.Extensions.Logging/issues/379#issuecomment-569544196
        /// </summary>
        /// <param name="services">Service provider</param>
        /// <returns>Service provider</returns>
        public static IServiceProvider AddWalrusLogging(this IServiceProvider provider)
        {
            WalrusLog.LoggerFactory = provider.GetService<ILoggerFactory>();
            return provider;
        }
    }
}
