using System.CommandLine;
using Walrus.Core;

namespace Walrus.CLI.Commands
{
    /// <summary>
    /// Command command implementation
    /// </summary>
    abstract class BaseCommand : IComposableCommand
    {
        /// <summary>
        /// Create a command
        /// </summary>
        /// <param name="walrus">Walrus service</param>
        protected BaseCommand(IWalrusService walrus)
        {
            Walrus = walrus;
            Command = new Command(Name, Description);
        }

        /// <summary>
        /// Walrus service context
        /// </summary>
        protected IWalrusService Walrus { get; }

        /// <inheritdoc />
        public abstract string Name { get; }

        /// <inheritdoc />
        public abstract string Description { get; }

        /// <inheritdoc />
        public Command Command { get; private set; }
    }
}
