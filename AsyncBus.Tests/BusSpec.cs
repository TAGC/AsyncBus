using Xunit;
using Shouldly;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace AsyncBus.Tests
{
    public sealed class BusSpec
    {
        private readonly IBus _bus;

        public BusSpec()
        {
            _bus = BusSetup.CreateBus();
        }

        [Fact]
        internal void Bus_Should_Allow_Registering_Subscriber_Returning_Void()
        {
            void Callback(string value) { }

            Should.NotThrow(() => _bus.SubscribeSync<string>(Callback));
        }

        [Fact]
        internal void Bus_Should_Allow_Registering_Subscriber_Returning_Task()
        {
            Task Callback(string value) => Task.CompletedTask;

            Should.NotThrow(() => _bus.Subscribe<string>(Callback));
        }

        [Fact]
        internal void Bus_Should_Allow_Registering_Subscriber_Taking_Cancellation_Token()
        {
            Task Callback(string value, CancellationToken cancellationToken) => Task.CompletedTask;

            Should.NotThrow(() => _bus.Subscribe<string>(Callback));
        }

        [Fact]
        internal async Task Subscriber_Should_Not_Be_Called_After_Corresponding_Subscription_Token_Disposed()
        {
            // GIVEN we register a callback to update a local variable.
            int? receivedValue = null;
            void Callback(int value) => receivedValue = value;
            var subscriptionToken = _bus.SubscribeSync<int>(Callback);

            // WHEN we publish a value.
            await _bus.Publish(3);

            // THEN the local variable should be this value.
            receivedValue.ShouldBe(3);
            receivedValue = null;

            // WHEN we dispose the subscription token.
            subscriptionToken.Dispose();

            // AND we publish another value.
            await _bus.Publish(5);

            // THEN the variable should not have been updated.
            receivedValue.ShouldBeNull();
        }

        [Fact]
        internal async Task Subscription_Should_Be_Covariant()
        {
            // GIVEN we register a callback that handles Parent objects.
            Parent receivedParent = null;
            void Callback(Parent parent) => receivedParent = parent;

            using (_bus.SubscribeSync<Parent>(Callback))
            {
                // WHEN we publish a child object.
                await _bus.Publish(new Child { Property = 5 });

                // THEN the subscriber should have received the published child.
                receivedParent.ShouldNotBeNull();
                receivedParent.Property.ShouldBe(5);
            }
        }

        [Fact]
        internal async Task Subscribers_Should_Be_Notified_In_Order_Of_Registration()
        {
            // GIVEN two subscribers register to the bus.
            var tcs = new TaskCompletionSource<object>();
            var secondCallbackCalled = false;

            Task CallbackA(int _) => tcs.Task;
            void CallbackB(int _) => secondCallbackCalled = true;

            using (_bus.Subscribe<int>(CallbackA))
            using (_bus.SubscribeSync<int>(CallbackB))
            {
                // WHEN we publish a message on the bus.
                var publicationTask = _bus.Publish(3);

                // THEN the second callback should not have been called.
                secondCallbackCalled.ShouldBeFalse();

                // WHEN we signal completion of the first callback.
                tcs.SetResult(null);

                // THEN the second callback should have been called.
                await publicationTask;
                secondCallbackCalled.ShouldBeTrue();
            }
        }

        [Fact]
        internal async Task Client_Should_Be_Able_To_Cancel_Message_Publication()
        {
            // GIVEN two subscribers register to the bus.
            var firstSubscriberNotifiedTcs = new TaskCompletionSource<object>();
            var firstSubscriberBlockTcs = new TaskCompletionSource<object>();
            var secondSubscriberNotified = false;

            async Task CallbackA(int _)
            {
                firstSubscriberNotifiedTcs.SetResult(null);
                await firstSubscriberBlockTcs.Task;
            }
            
            void CallbackB(int _) => secondSubscriberNotified = true;

            using (_bus.Subscribe<int>(CallbackA))
            using (_bus.SubscribeSync<int>(CallbackB))
            {
                // WHEN we publish a message with a cancellation token.
                var cts = new CancellationTokenSource();
                var publicationTask = _bus.Publish(3, cts.Token);
                await firstSubscriberNotifiedTcs.Task;

                // THEN the second callback should not have been called.
                secondSubscriberNotified.ShouldBeFalse();

                // WHEN we request cancellation of the publication.
                cts.Cancel();

                // AND we signal completion of the first callback.
                firstSubscriberBlockTcs.SetResult(null);

                // THEN the second callback should still not have been called.
                // NOTE: done this way because Should.ThrowAsync does not work for OperationCanceledException
                try
                {
                    await publicationTask;
                    throw new Exception("Task was not canceled as expected");
                }
                catch (OperationCanceledException)
                {
                    // Test successful.
                }

                secondSubscriberNotified.ShouldBeFalse();
            }
        }

        [Fact]
        internal async Task Cancellation_Tokens_Should_Be_Passed_To_Subscribers()
        {
            // GIVEN a subscriber that accepts a cancellation token is registered to the bus.
            var cancellationRequested = false;
            var tcs = new TaskCompletionSource<object>();
            async Task Callback(int _, CancellationToken cancellationToken)
            {
                await tcs.Task;
                cancellationRequested = cancellationToken.IsCancellationRequested;
            }

            using (_bus.Subscribe<int>(Callback))
            {
                // WHEN we publish a message with a cancellation token.
                var cts = new CancellationTokenSource();
                var publicationTask = _bus.Publish(3, cts.Token);

                // AND we signal cancellation.
                cts.Cancel();

                // THEN the subscriber should be notified of that cancellation.
                tcs.SetResult(null);
                await publicationTask;
                cancellationRequested.ShouldBeTrue();
            }

        }

        [Fact]
        internal async Task Exceptions_Should_Propagate_From_Subscribers_To_Publication_Task()
        {
            // GIVEN a subscriber that throws an exception when notified.
            void Callback(int _) => throw new ArgumentException("All numbers are terrible.");

            using (_bus.SubscribeSync<int>(Callback))
            {
                // EXCEPT an exception is thrown when we publish an integer on the bus.
                var exception = await Should.ThrowAsync<ArgumentException>(_bus.Publish(3));
                exception.Message.ShouldBe("All numbers are terrible.");
            }
        }

        private class Parent
        {
            public int Property { get; set; }
        }

        private class Child : Parent
        {
        }
    }
}