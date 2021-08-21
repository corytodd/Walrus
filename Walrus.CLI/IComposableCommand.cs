namespace Walrus.CLI
{
    using System.CommandLine;

    /// <summary>
    /// Basic unit of work in the CLI. Each command requires a Name
    /// which is the invocation verb for the CLI. The description is 
    /// the friendly text shown on the help menu. The Command ref
    /// is the underlying implementation.
    /// </summary>
    interface IComposableCommand
    {
        /// <summary>
        /// Invocation verb
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Help text
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Command line implementation
        /// </summary>
        public Command Command { get; }
    }
}
