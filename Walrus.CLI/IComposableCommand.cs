namespace Walrus.CLI
{
    using System.CommandLine;

    /// <summary>
    ///     Basic unit of work in the CLI. Each command requires a Name
    ///     which is the invocation verb for the CLI. The description is
    ///     the friendly text shown on the help menu. The Command ref
    ///     is the underlying implementation.
    /// </summary>
    internal interface IComposableCommand
    {
        /// <summary>
        ///     Invocation verb
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Help text
        /// </summary>
        string Description { get; }

        /// <summary>
        ///     Command line implementation
        /// </summary>
        Command Command { get; }
    }
}