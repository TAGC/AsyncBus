using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncBus
{
    /// <summary>
    /// Represents a type of <see cref="ISubscription" /> that notifies observers when messages are received of the correct
    /// type.
    /// </summary>
    /// <typeparam name="T">The type of messages to handle.</typeparam>
    internal sealed class ObservableSubscription<T> : ISubscription, IObservable<T>
    {
        private bool _disposed;
        private IObserver<T> _observer;

        /// <inheritdoc />
        public event EventHandler Disposed;

        /// <inheritdoc />
        public bool CanProcessMessage(object message) => message is T && !_disposed;

        /// <inheritdoc />
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            Disposed?.Invoke(this, EventArgs.Empty);
            _observer = null;
            _disposed = true;
        }

        /// <inheritdoc />
        public Task ProcessMessage(object message, CancellationToken cancellationToken)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            _observer?.OnNext((T)message);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        /// <exception cref="InvalidOperationException">This instance already has an observer.</exception>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (_observer != null)
            {
                throw new InvalidOperationException(
                    "This observable already has an observer and does not support multiple observers");
            }

            _observer = observer;
            return this;
        }
    }
}