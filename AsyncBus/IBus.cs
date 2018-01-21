using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncBus
{
    public interface IBus
    {
        Task Publish(object message, CancellationToken cancellationToken = default);

        IDisposable Subscribe<T>(Func<T, CancellationToken, Task> callback);

        IDisposable Subscribe<T>(Func<T, Task> callback);
        
        IDisposable SubscribeSync<T>(Action<T> callback);
    }
}