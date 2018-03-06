using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncBus
{
    /// <summary>
    /// Represents a message bus that allows for asynchronous message publication and handling.
    /// </summary>
    public interface IBus
    {
        /// <summary>
        /// Returns an <see cref="IObservable{T}" /> that allows clients to observe all messages of type
        /// <typeparamref name="T" /> published on the bus.
        /// <para />
        /// Each <c>IObservable</c> returned by this method supports only one subscriber. Callers should instead create additional
        /// instances of <c>IObservable</c> through this method if required.
        /// </summary>
        /// <typeparam name="T">The type of message to observe.</typeparam>
        /// <returns>An observable.</returns>
        IObservable<T> Observe<T>();

        /// <summary>
        /// Publishes a message on the bus.
        /// <para />
        /// Any subscribers registered to the runtime type of <paramref name="message" /> or a supertype
        /// will be notified of the message.
        /// </summary>
        /// <param name="message">The message to publish on the bus.</param>
        /// <param name="cancellationToken">A token that can be used to cancel publication of the message.</param>
        /// <returns>
        /// A task representing all computation done by subscribers to the message. Awaiting this task will
        /// prevent continuation in the current context until all subscribers have finishing handling the
        /// message. If any subscribers throw an exception, it will be propagated to the returned task.
        /// </returns>
        Task Publish(object message, CancellationToken cancellationToken = default);

        /// <summary>
        /// Registers a subscriber to messages of type <typeparamref name="T" />.
        /// <para />
        /// Message subscription is contravariant; subscribers of messages of type <typeparamref name="T" />
        /// will also receive all messages that are a subclass of that type.
        /// <para />
        /// Subscribers will be passed both the message and a cancellation token that clients may use to signal
        /// cancellation of the message publication.
        /// </summary>
        /// <param name="callback">
        /// The asynchronous action to perform when an appropriate message is published on the bus.
        /// </param>
        /// <typeparam name="T">The type of message to subscribe to.</typeparam>
        /// <returns>A token that can be disposed to unregister this subscriber.</returns>
        IDisposable Subscribe<T>(Func<T, CancellationToken, Task> callback);

        /// <summary>
        /// Registers a subscriber to messages of type <typeparamref name="T" />.
        /// <para />
        /// Message subscription is contravariant; subscribers of messages of type <typeparamref name="T" />
        /// will also receive all messages that are a subclass of that type.
        /// </summary>
        /// <param name="callback">
        /// The asynchronous action to perform when an appropriate message is published on the bus.
        /// </param>
        /// <typeparam name="T">The type of message to subscribe to.</typeparam>
        /// <returns>A token that can be disposed to unregister this subscriber.</returns>
        IDisposable Subscribe<T>(Func<T, Task> callback);

        /// <summary>
        /// Registers a subscriber to messages of type <typeparamref name="T" />.
        /// <para />
        /// Message subscription is contravariant; subscribers of messages of type <typeparamref name="T" />
        /// will also receive all messages that are a subclass of that type.
        /// </summary>
        /// <param name="callback">
        /// The synchronous action to perform when an appropriate message is published on the bus.
        /// </param>
        /// <typeparam name="T">The type of message to subscribe to.</typeparam>
        /// <returns>A token that can be disposed to unregister this subscriber.</returns>
        IDisposable SubscribeSync<T>(Action<T> callback);
    }
}