using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncBus
{
    public interface IAsyncBus
    {
        Task Publish(object message);

        IDisposable Subscribe<T>(Func<T, CancellationToken, Task> callback);

        IDisposable Subscribe<T>(Func<T, Task> callback);
        
        IDisposable Susbcribe<T>(Action<T> callback);
    }
}