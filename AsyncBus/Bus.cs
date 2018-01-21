using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncBus
{
    public sealed class Bus : IBus
    {
        public Task Publish(object message, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public IDisposable Subscribe<T>(Func<T, Task> callback)
        {
            throw new NotImplementedException();
        }

        public IDisposable Subscribe<T>(Func<T, CancellationToken, Task> callback)
        {
            throw new NotImplementedException();
        }

        public IDisposable SubscribeSync<T>(Action<T> callback)
        {
            throw new NotImplementedException();
        }
    }
}