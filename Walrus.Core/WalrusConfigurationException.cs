namespace Walrus.Core
{
    using System;
    
    /// <inheritdoc />
    [Serializable]
    public class WalrusConfigurationException : Exception
    {
        /// <inheritdoc />
        public WalrusConfigurationException() { }

        /// <inheritdoc />
        public WalrusConfigurationException(string message) : base(message) { }

        /// <inheritdoc />
        public WalrusConfigurationException(string message, System.Exception inner) : base(message, inner) { }

        /// <inheritdoc />
        protected WalrusConfigurationException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}