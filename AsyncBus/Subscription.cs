using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncBus
{
    /// <inheritdoc />
    internal sealed class Subscription<T> : ISubscription
    {
        private readonly Func<T, CancellationToken, Task> _callback;

        private bool _disposed;

        public Subscription(Func<T, CancellationToken, Task> callback)
        {
            _callback = callback ?? throw new ArgumentNullException(nameof(_callback));
        }

        /// <inheritdoc />
        public event EventHandler Disposed;

        /// <inheritdoc />
        public bool CanProcessMessage(object message)
            => message is T && !_disposed;

        /// <inheritdoc />
        public Task ProcessMessage(object message, CancellationToken cancellationToken)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            return _callback((T)message, cancellationToken);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            Disposed?.Invoke(this, EventArgs.Empty);
            _disposed = true;
        }
    }
}