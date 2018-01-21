using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncBus
{
    internal interface ISubscription : IDisposable
    {
        event EventHandler Disposed;

        bool CanProcessMessage(object message);

        Task ProcessMessage(object message, CancellationToken cancellationToken);
    }
}