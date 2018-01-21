using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncBus
{
    internal sealed class Subscription<T> : ISubscription
    {
        private readonly Func<T, CancellationToken, Task> _callback;

        private bool disposed;

        public Subscription(Func<T, CancellationToken, Task> callback)
        {
            _callback = callback ?? throw new ArgumentNullException(nameof(_callback));
        }

        public event EventHandler Disposed;

        public bool CanProcessMessage(object message)
            => message is T && !disposed;

        public Task ProcessMessage(object message, CancellationToken cancellationToken)
        {
            if (disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            return _callback((T)message, cancellationToken);
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            Disposed?.Invoke(this, EventArgs.Empty);
            disposed = true;
        }
    }
}