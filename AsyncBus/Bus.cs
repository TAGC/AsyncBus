using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncBus
{
    public class Bus : IAsyncBus
    {
        public Task Publish(object message)
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

        public IDisposable Susbcribe<T>(Action<T> callback)
        {
            throw new NotImplementedException();
        }
    }
}