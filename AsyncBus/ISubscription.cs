using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncBus
{
    /// <summary>
    /// Represents a disposable subscription to a message bus.
    /// </summary>
    internal interface ISubscription : IDisposable
    {
        /// <summary>
        /// Raised when this subscription is disposed.
        /// </summary>
        event EventHandler Disposed;

        /// <summary>
        /// Returns whether this subscription is suitable for handling a particular
        /// message published on a message bus.
        /// </summary>
        /// <param name="message">The published message.</param>
        /// <returns><c>true</c> if the subscription can handle the message; otherwise, <c>false</c></returns>
        bool CanProcessMessage(object message);

        /// <summary>
        /// Attempts to handle a message published on the message bus.
        /// </summary>
        /// <param name="message">The published message.</param>
        /// <param name="cancellationToken">
        /// A token that the client may use to signal cancellation of the message
        /// publication.
        /// </param>
        /// <returns>A task representing the handling performed by the subscriber on the message.</returns>
        Task ProcessMessage(object message, CancellationToken cancellationToken);
    }
}