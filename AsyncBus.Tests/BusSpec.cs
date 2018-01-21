using Xunit;
using AsyncBus;
using System;
using System.Threading.Tasks;

namespace AsyncBus.Tests
{
    public class BusSpec
    {
        private readonly IAsyncBus _bus;

        public BusSpec()
        {
            _bus = new Bus();
        }

        [Fact]
        internal void Bus_Should_Allow_Registering_Callback_Returning_Void()
        {
            throw new NotImplementedException();
        }

        [Fact]
        internal void Bus_Should_Allow_Registering_Callback_Returning_Task()
        {
            throw new NotImplementedException();
        }

        [Fact]
        internal void Bus_Should_Allow_Registering_Callback_Taking_Cancellation_Token()
        {
            throw new NotImplementedException();
        }

        [Fact]
        internal void Subscriber_Should_Not_Be_Called_After_Corresponding_Subscription_Token_Disposed()
        {
            throw new NotImplementedException();
        }

        [Fact]
        internal async Task Subscription_Should_Be_Covariant()
        {
            throw new NotImplementedException();
        }

        [Fact]
        internal async Task Client_Should_Be_Able_To_Cancel_Message_Publication()
        {
            throw new NotImplementedException();
        }

        [Fact]
        internal async Task Cancellation_Tokens_Should_Be_Passed_To_Subscribers()
        {
            throw new NotImplementedException();
        }
    }
}