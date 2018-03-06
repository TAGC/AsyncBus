using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace AsyncBus.Tests
{
    using ObservableSetup = Func<IObservable<int>, IObservable<int>>;

    public class ObservableSpec
    {
        private readonly IBus _bus;

        public ObservableSpec()
        {
            _bus = BusSetup.CreateBus();
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static IEnumerable<object[]> RxExamples
        {
            get
            {
                yield return new object[]
                {
                    new[] { 1, 2, 3, 4, 5, 6 },
                    new ObservableSetup(observable => observable),
                    new[] { 1, 2, 3, 4, 5, 6 }
                };

                yield return new object[]
                {
                    new[] { 1, 2, 2, 2, 3, 2, 2, 4 },
                    new ObservableSetup(observable => observable.Distinct()),
                    new[] { 1, 2, 3, 4 }
                };

                yield return new object[]
                {
                    new[] { 1, 2, 2, 2, 3, 2, 2, 4 },
                    new ObservableSetup(observable => observable.DistinctUntilChanged()),
                    new[] { 1, 2, 3, 2, 4 }
                };

                yield return new object[]
                {
                    new[] { 1, 2, 3, 4 },
                    new ObservableSetup(observable => observable.Scan(0, (x, y) => x + y)),
                    new[] { 1, 3, 6, 10 }
                };

                yield return new object[]
                {
                    new[] { 1, 2, 3, 4 },
                    new ObservableSetup(observable => observable.Select(x => x * 2).Select(x => x - 5)),
                    new[] { -3, -1, 1, 3 }
                };
            }
        }

        [Fact]
        internal async Task Bus_Should_Support_Multiple_Observers()
        {
            var input = new[] { 1, 2, 3, 4 };
            var outputA = new List<int>();
            var outputB = new List<int>();

            // GIVEN the bus has two observers
            using (_bus.Observe<int>().Subscribe(outputA.Add))
            using (_bus.Observe<int>().Subscribe(outputB.Add))
            {
                // WHEN we publish a sequence of messages.
                foreach (var message in input)
                {
                    await _bus.Publish(message);
                }

                // THEN both observers should have received the messages.
                outputA.ShouldBe(input);
                outputB.ShouldBe(input);
            }
        }

        [Theory]
        [MemberData(nameof(RxExamples))]
        internal async Task Bus_Should_Support_Observation(int[] input, ObservableSetup setup, int[] expectedOutput)
        {
            var output = new List<int>();

            using (setup(_bus.Observe<int>()).Subscribe(output.Add))
            {
                foreach (var message in input)
                {
                    await _bus.Publish(message);
                }

                output.ShouldBe(expectedOutput);
            }
        }

        [Fact]
        internal void Observables_Returned_By_Bus_Should_Throw_Exception_On_Resubscription()
        {
            void ExampleCallback(int _)
            {
            }

            var observable = _bus.Observe<int>();
            Should.NotThrow(() => observable.Subscribe(ExampleCallback));
            Should.Throw<InvalidOperationException>(() => observable.Subscribe(ExampleCallback));
        }

        [Fact]
        internal async Task Observers_Should_Not_Receive_Messages_After_Disposing_Subscription()
        {
            // GIVEN the bus has two observers.
            var outputA = new List<int>();
            var outputB = new List<int>();
            var subscriptionA = _bus.Observe<int>().Subscribe(outputA.Add);
            var subscriptionB = _bus.Observe<int>().Subscribe(outputB.Add);

            // EXPECT that observers only receive updates until they dispose their subscription.
            await _bus.Publish(1);
            outputA.ShouldBe(new[] { 1 });
            outputB.ShouldBe(new[] { 1 });

            subscriptionA.Dispose();
            await _bus.Publish(2);
            outputA.ShouldBe(new[] { 1 });
            outputB.ShouldBe(new[] { 1, 2 });

            subscriptionB.Dispose();
            await _bus.Publish(3);
            outputA.ShouldBe(new[] { 1 });
            outputB.ShouldBe(new[] { 1, 2 });
        }
    }
}