using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncBus
{
    /// <inheritdoc />
    internal sealed class Bus : IBus
    {
        private readonly List<ISubscription> _subscriptions;

        public Bus()
        {
            _subscriptions = new List<ISubscription>();
        }

        /// <inheritdoc />
        public async Task Publish(object message, CancellationToken cancellationToken = default)
        {
            foreach (var subscription in _subscriptions)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (subscription.CanProcessMessage(message))
                {
                    await subscription.ProcessMessage(message, cancellationToken);
                }
            }
        }

        /// <inheritdoc />
        public IDisposable Subscribe<T>(Func<T, CancellationToken, Task> callback)
        {
            var subscription = new Subscription<T>(callback);
            subscription.Disposed += (s, e) => _subscriptions.Remove(subscription);
            _subscriptions.Add(subscription);

            return subscription;
        }

        /// <inheritdoc />
        public IDisposable Subscribe<T>(Func<T, Task> callback)
            => Subscribe<T>((message, cancellationToken) => callback(message));

        /// <inheritdoc />
        public IDisposable SubscribeSync<T>(Action<T> callback)
            => Subscribe<T>(message => Task.Run(() => callback(message)));
    }
}