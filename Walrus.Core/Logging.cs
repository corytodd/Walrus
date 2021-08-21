using Microsoft.Extensions.Logging;
using System;

namespace Walrus.Core
{
    /// <summary>
    ///     Logging implementation that is framework agnostic
    ///     Credit to https://github.com/NLog/NLog.Extensions.Logging/issues/379
    /// </summary>
    public static class WalrusLog
    {
        private static readonly WalrusLogger WalrusLoggerImpl = new WalrusLogger();
        public static ILogger Logger => WalrusLoggerImpl;

        public static ILoggerFactory LoggerFactory
        {
            get => WalrusLoggerImpl.Factory;
            set => WalrusLoggerImpl.Factory = value;
        }

        /// <summary>
        ///     Logger implementation redirects to Microsoft.Extensions.Logging
        /// </summary>
        private class WalrusLogger : ILogger
        {
            private ILogger? _logger;
            private ILoggerFactory? _loggerFactory;

            /// <summary>
            ///     Get or Set the log factory for your process using this library
            ///     <example>
            ///         WalrusLog.LoggerFactory = services.BuildServiceProvider().GetService{ILoggerFactory}();;
            ///     </example>
            /// </summary>
            public ILoggerFactory Factory
            {
                get => _loggerFactory!;
                set
                {
                    _loggerFactory = value;
                    _logger = null;
                }
            }

            // ReSharper disable once ConstantNullCoalescingCondition
            private ILogger LoggerImpl => (_logger ?? _loggerFactory?.CreateLogger("WalrusLogger"))!;

            /// <inheritdoc />
            IDisposable ILogger.BeginScope<TState>(TState state)
            {
                return LoggerImpl?.BeginScope(state)!;
            }

            /// <inheritdoc />
            bool ILogger.IsEnabled(LogLevel logLevel)
            {
                return LoggerImpl?.IsEnabled(logLevel) ?? false;
            }

            /// <inheritdoc />
            void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
                Func<TState, Exception, string> formatter)
            {
                LoggerImpl?.Log(logLevel, eventId, state, exception, formatter);
            }
        }
    }
}