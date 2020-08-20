// -----------------------------------------------------------------
// <copyright file="SchedulerTests.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Scheduling.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Fibula.Common.TestingUtilities;
    using Fibula.Scheduling;
    using Fibula.Scheduling.Contracts.Abstractions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Serilog;

    /// <summary>
    /// Tests for the <see cref="Scheduler"/> class.
    /// </summary>
    [TestClass]
    public class SchedulerTests
    {
        /// <summary>
        /// Checks <see cref="Scheduler"/> initialization.
        /// </summary>
        [TestMethod]
        public void Scheduler_Initialization()
        {
            Mock<ILogger> loggerMock = new Mock<ILogger>();

            ExceptionAssert.Throws<ArgumentNullException>(() => new Scheduler(null), $"Value cannot be null. (Parameter 'logger')");

            // use a non default reference time.
            new Scheduler(loggerMock.Object);
        }

        /// <summary>
        /// Checks that <see cref="Scheduler.ScheduleEvent(IEvent, TimeSpan?, bool)"/> throws when needed.
        /// </summary>
        [TestMethod]
        public void ScheduleEvent_Throws_WhenBad()
        {
            const uint RequestorId = 1;

            TimeSpan oneMillisecondDelayTime = TimeSpan.FromMilliseconds(1);
            TimeSpan twoSecondsDelayTime = TimeSpan.FromSeconds(2);

            Scheduler scheduler = this.SetupSchedulerWithLoggerMock();

            ExceptionAssert.Throws<ArgumentNullException>(() => scheduler.ScheduleEvent(null, oneMillisecondDelayTime), $"Value cannot be null. (Parameter 'eventToSchedule')");

            Mock<IEvent> eventMock = new Mock<IEvent>();

            ExceptionAssert.Throws<ArgumentException>(() => scheduler.ScheduleEvent(eventMock.Object, oneMillisecondDelayTime), $"Argument must be of type {nameof(BaseEvent)}. (Parameter 'eventToSchedule')");

            Mock<BaseEvent> bEventMock = new Mock<BaseEvent>(RequestorId);

            // schedule twice
            scheduler.ScheduleEvent(bEventMock.Object, twoSecondsDelayTime);

            ExceptionAssert.Throws<ArgumentException>(() => scheduler.ScheduleEvent(bEventMock.Object), $"The event is already scheduled. (Parameter 'eventToSchedule')");
        }

        /// <summary>
        /// Checks that <see cref="Scheduler.CancelEvent(IEvent)"/> does what it should.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task Cancelling_SingleEvent()
        {
            const uint RequestorId = 1;
            const int ExpectedCounterValueBeforeRun = 0;
            const int ExpectedCounterValueAfterRun = 0;

            TimeSpan overheadDelay = TimeSpan.FromMilliseconds(100);
            TimeSpan twoSecondsTimeSpan = TimeSpan.FromSeconds(2);
            TimeSpan threeSecondsTimeSpan = TimeSpan.FromSeconds(3);

            var scheduledEventFiredCounter = 0;

            Mock<BaseEvent> bEventMockForScheduled = new Mock<BaseEvent>(RequestorId);

            bEventMockForScheduled.SetupGet(e => e.CanBeCancelled).Returns(true);

            Scheduler scheduler = this.SetupSchedulerWithLoggerMock();

            using CancellationTokenSource cts = new CancellationTokenSource();

            scheduler.EventFired += (sender, eventArgs) =>
            {
                // test that sender is the same scheduler instance, while we're here.
                Assert.AreEqual(scheduler, sender);

                // check that event has a reference.
                Assert.IsNotNull(eventArgs?.Event);

                if (eventArgs.Event == bEventMockForScheduled.Object)
                {
                    scheduledEventFiredCounter++;
                }
            };

            // start the scheduler.
            Task schedulerTask = scheduler.RunAsync(cts.Token);

            // fire a scheduled event that shall be fired only after some seconds.
            scheduler.ScheduleEvent(bEventMockForScheduled.Object, twoSecondsTimeSpan);

            // delay for 100 ms (to account for setup overhead and multi threading) and check that the counter has NOT gone up for scheduled
            await Task.Delay(overheadDelay).ContinueWith(prev =>
            {
                Assert.AreEqual(ExpectedCounterValueBeforeRun, scheduledEventFiredCounter, $"Scheduled events counter does not match: Expected {ExpectedCounterValueBeforeRun}, got {scheduledEventFiredCounter}.");
            });

            // cancel this event.
            scheduler.CancelEvent(bEventMockForScheduled.Object);

            // delay for three seconds and check that the counter has NOT gone up for scheduled.
            await Task.Delay(threeSecondsTimeSpan).ContinueWith(prev =>
            {
                Assert.AreEqual(ExpectedCounterValueAfterRun, scheduledEventFiredCounter, $"Scheduled events counter does not match: Expected {ExpectedCounterValueAfterRun}, got {scheduledEventFiredCounter}.");
            });
        }

        /// <summary>
        /// Checks that event expedition works as intented.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task Event_Expedition_Works()
        {
            const uint RequestorId = 1;
            const int ExpectedCounterValueBeforeRun = 0;
            const int ExpectedCounterValueAfterRun = 1;
            const int ExpectedCounterValueAfterWaitingMore = 1;
            const int ExpectedCounterValueAfterWaitingEvenMore = 2;

            TimeSpan overheadDelay = TimeSpan.FromMilliseconds(100);
            TimeSpan twoSecondsTimeSpan = TimeSpan.FromSeconds(2);
            TimeSpan threeSecondsTimeSpan = TimeSpan.FromSeconds(3);
            TimeSpan fiveSecondsTimeSpan = TimeSpan.FromSeconds(5);

            var scheduledEventFiredCounter = 0;

            Mock<BaseEvent> bEventMockForScheduled = new Mock<BaseEvent>(RequestorId);
            ConcreteMockedEvent testConcreteEvent = new ConcreteMockedEvent(RequestorId, true);

            Scheduler scheduler = this.SetupSchedulerWithLoggerMock();

            using CancellationTokenSource cts = new CancellationTokenSource();

            scheduler.EventFired += (sender, eventArgs) =>
            {
                // test that sender is the same scheduler instance, while we're here.
                Assert.AreEqual(scheduler, sender);

                // check that event has a reference.
                Assert.IsNotNull(eventArgs?.Event);

                scheduledEventFiredCounter++;
            };

            // start the scheduler.
            Task schedulerTask = scheduler.RunAsync(cts.Token);

            // fire a scheduled event that shall be fired only after some seconds.
            scheduler.ScheduleEvent(bEventMockForScheduled.Object, fiveSecondsTimeSpan);
            scheduler.ScheduleEvent(testConcreteEvent, threeSecondsTimeSpan);

            // delay for 100 ms (to account for setup overhead and multi threading) and check that the counter has NOT gone up for scheduled
            await Task.Delay(overheadDelay).ContinueWith(prev =>
            {
                Assert.AreEqual(ExpectedCounterValueBeforeRun, scheduledEventFiredCounter, $"Expected events counter to be {ExpectedCounterValueBeforeRun} before first run but got {scheduledEventFiredCounter}.");
            });

            // expedite this event.
            var expedited = testConcreteEvent.Expedite();

            Assert.IsTrue(expedited, "Expected event to confirm being expedited.");

            // delay for two seconds and check that the counter has gone up for scheduled, meaning the event actually got expedited.
            await Task.Delay(twoSecondsTimeSpan).ContinueWith(prev =>
            {
                Assert.AreEqual(ExpectedCounterValueAfterRun, scheduledEventFiredCounter, $"Expected events counter to be {ExpectedCounterValueAfterRun} after first run but got {scheduledEventFiredCounter}.");
            });

            // delay for two more seconds and check that the counter has gone NOT up, meaning the event didn't run again.
            await Task.Delay(twoSecondsTimeSpan).ContinueWith(prev =>
            {
                Assert.AreEqual(ExpectedCounterValueAfterWaitingMore, scheduledEventFiredCounter, $"Expected events counter to be {ExpectedCounterValueAfterRun} after waiting more but got {scheduledEventFiredCounter}.");
            });

            // delay for two more seconds and check that the counter has gone up, since the original 5 second event finally ran.
            await Task.Delay(twoSecondsTimeSpan).ContinueWith(prev =>
            {
                Assert.AreEqual(ExpectedCounterValueAfterWaitingEvenMore, scheduledEventFiredCounter, $"Expected events counter to be {ExpectedCounterValueAfterWaitingEvenMore} after waiting even more but got {scheduledEventFiredCounter}.");
            });
        }

        /// <summary>
        /// Checks that <see cref="Scheduler.CancelAllFor"/> does what it should.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task Cancelling_AllEventsFor()
        {
            TimeSpan overheadDelay = TimeSpan.FromMilliseconds(100);
            TimeSpan oneSecondDelay = TimeSpan.FromSeconds(1);
            TimeSpan threeSecondsTimeSpan = TimeSpan.FromSeconds(3);

            const uint anyRequestorId = 100u;
            const uint anyOtherRequestorId = 200u;
            const int ExpectedCounterValueBeforeRun = 0;
            const int ExpectedCounterValueAfterRun = 1;
            const int ExpectedCounterValueBeforeSecondRun = 1;
            const int ExpectedCounterValueAfterSecondRun = 2;

            var scheduledEventFiredCounter = 0;

            Mock<BaseEvent> bEventMock1 = new Mock<BaseEvent>(anyRequestorId);
            Mock<BaseEvent> bEventMock2 = new Mock<BaseEvent>(anyRequestorId);
            Mock<BaseEvent> bEventMock3 = new Mock<BaseEvent>(anyOtherRequestorId);
            Mock<BaseEvent> bEventMockUncancellable = new Mock<BaseEvent>(anyRequestorId);
            ConcreteMockedEvent testConcreteEvent = new ConcreteMockedEvent(anyRequestorId, true);

            bEventMock1.SetupGet(e => e.CanBeCancelled).Returns(true);
            bEventMock2.SetupGet(e => e.CanBeCancelled).Returns(true);
            bEventMock3.SetupGet(e => e.CanBeCancelled).Returns(true);
            bEventMockUncancellable.SetupGet(e => e.CanBeCancelled).Returns(false);

            Scheduler scheduler = this.SetupSchedulerWithLoggerMock();

            using CancellationTokenSource cts = new CancellationTokenSource();

            scheduler.EventFired += (sender, eventArgs) =>
            {
                // test that sender is the same scheduler instance, while we're here.
                Assert.AreEqual(scheduler, sender, "Test missconfigured, sender is not the the supposed one.");

                // check that event has a reference.
                Assert.IsNotNull(eventArgs?.Event, "Expected to have a non-null reference in event.");

                scheduledEventFiredCounter++;
            };

            // start the scheduler.
            Task schedulerTask = scheduler.RunAsync(cts.Token);

            // fire a scheduled event that shall be fired only after one second.
            scheduler.ScheduleEvent(bEventMock1.Object, oneSecondDelay);
            scheduler.ScheduleEvent(bEventMock2.Object, oneSecondDelay);
            scheduler.ScheduleEvent(bEventMock3.Object, oneSecondDelay);

            // delay for 100 ms (to account for setup overhead and multi threading) and check that the counter has NOT gone up for scheduled
            await Task.Delay(overheadDelay).ContinueWith(prev =>
            {
                Assert.AreEqual(ExpectedCounterValueBeforeRun, scheduledEventFiredCounter, $"Expected scheduler's events counter {scheduledEventFiredCounter} to match {ExpectedCounterValueBeforeRun} before the first run.");
            });

            // cancel this event.
            scheduler.CancelAllFor(anyRequestorId);

            // delay for three seconds and check that the counter has NOT gone up for scheduled.
            await Task.Delay(threeSecondsTimeSpan).ContinueWith(prev =>
            {
                Assert.AreEqual(ExpectedCounterValueAfterRun, scheduledEventFiredCounter, $"Expected scheduler's events counter {scheduledEventFiredCounter} to match {ExpectedCounterValueAfterRun} after the first run.");
            });

            scheduler.ScheduleEvent(bEventMockUncancellable.Object, oneSecondDelay);
            scheduler.ScheduleEvent(testConcreteEvent, oneSecondDelay);

            // delay for 100 ms (to account for setup overhead and multi threading) and check that the counter has NOT gone up for scheduled
            await Task.Delay(overheadDelay).ContinueWith(prev =>
            {
                Assert.AreEqual(ExpectedCounterValueBeforeSecondRun, scheduledEventFiredCounter, $"Expected scheduler's events counter {scheduledEventFiredCounter} to match {ExpectedCounterValueBeforeSecondRun} before the second run.");
            });

            // cancel only the concrete implementation event.
            scheduler.CancelAllFor(anyRequestorId, typeof(ConcreteMockedEvent));

            // delay for three seconds and check that the time for execution has passed.
            await Task.Delay(threeSecondsTimeSpan).ContinueWith(prev =>
            {
                Assert.AreEqual(ExpectedCounterValueAfterSecondRun, scheduledEventFiredCounter, $"Expected scheduler's events counter {scheduledEventFiredCounter} to match {ExpectedCounterValueAfterSecondRun} after the second run.");
            });
        }

        /// <summary>
        /// Checks that <see cref="Scheduler.EventFired"/> gets fired when an event is scheduled.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task OnEventFired_IsCalled()
        {
            const uint RequestorId = 1;
            const int ExpectedCounterValueBeforeRun = 0;
            const int ExpectedCounterValueAfterRun = 1;

            TimeSpan twoSecondsTimeSpan = TimeSpan.FromSeconds(2);
            TimeSpan overheadDelay = TimeSpan.FromMilliseconds(100);

            Mock<BaseEvent> bEventMockForInmediate = new Mock<BaseEvent>(RequestorId);
            Mock<BaseEvent> bEventMockForScheduled = new Mock<BaseEvent>(RequestorId);

            Scheduler scheduler = this.SetupSchedulerWithLoggerMock();

            var inmediateEventFiredCounter = 0;
            var scheduledEventFiredCounter = 0;

            using CancellationTokenSource cts = new CancellationTokenSource();

            scheduler.EventFired += (sender, eventArgs) =>
            {
                // test that sender is the same scheduler instance, while we're here.
                Assert.AreEqual(scheduler, sender);

                // check that event has a reference.
                Assert.IsNotNull(eventArgs?.Event);

                if (eventArgs.Event == bEventMockForInmediate.Object)
                {
                    inmediateEventFiredCounter++;
                }
                else if (eventArgs.Event == bEventMockForScheduled.Object)
                {
                    scheduledEventFiredCounter++;
                }
            };

            // start the scheduler.
            Task schedulerTask = scheduler.RunAsync(cts.Token);

            // fire a scheduled event that shall be fired only after some seconds.
            scheduler.ScheduleEvent(bEventMockForScheduled.Object, twoSecondsTimeSpan);

            // delay for 100 ms (to account for setup overhead and multi threading) and check that the counter has NOT gone up for scheduled
            await Task.Delay(overheadDelay).ContinueWith(prev =>
            {
                Assert.AreEqual(ExpectedCounterValueBeforeRun, scheduledEventFiredCounter, $"Scheduled events counter does not match: Expected {ExpectedCounterValueBeforeRun}, got {scheduledEventFiredCounter}.");
            });

            // fire the inmediate event, which should be run asap.
            scheduler.ScheduleEvent(bEventMockForInmediate.Object);

            // delay for 500 ms and check that the counter has gone up.
            await Task.Delay(overheadDelay).ContinueWith(prev =>
            {
                Assert.AreEqual(ExpectedCounterValueAfterRun, inmediateEventFiredCounter, $"Inmediate events counter does not match: Expected {ExpectedCounterValueAfterRun}, got {inmediateEventFiredCounter}.");
            });

            // delay for the remaining seconds and check that the counter has gone up for scheduled.
            await Task.Delay(twoSecondsTimeSpan).ContinueWith(prev =>
            {
                Assert.AreEqual(ExpectedCounterValueAfterRun, scheduledEventFiredCounter, $"Scheduled events counter does not match: Expected {ExpectedCounterValueAfterRun}, got {scheduledEventFiredCounter}.");
            });
        }

        /// <summary>
        /// Helper method used to setup a <see cref="Scheduler"/> instance.
        /// </summary>
        /// <returns>The scheduler instance.</returns>
        private Scheduler SetupSchedulerWithLoggerMock()
        {
            Mock<ILogger> schedulerLoggerMock = new Mock<ILogger>();

            schedulerLoggerMock.Setup(l => l.ForContext<Scheduler>()).Returns(schedulerLoggerMock.Object);

            return new Scheduler(schedulerLoggerMock.Object);
        }

        /// <summary>
        /// Internal class used for testing.
        /// </summary>
        internal class ConcreteMockedEvent : BaseEvent
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ConcreteMockedEvent"/> class.
            /// </summary>
            /// <param name="requestorId">A requestor id to pass down to the base class.</param>
            /// <param name="canBeCancelled">A value indicating whether this event can be cancelled.</param>
            public ConcreteMockedEvent(uint requestorId, bool canBeCancelled)
                : base(requestorId)
            {
                this.CanBeCancelled = canBeCancelled;
            }

            /// <summary>
            /// Gets a string representing this event's type.
            /// </summary>
            public override string EventType => nameof(ConcreteMockedEvent);

            /// <summary>
            /// Gets or sets a value indicating whether the event can be cancelled.
            /// </summary>
            public override bool CanBeCancelled { get; protected set; }

            /// <summary>
            /// Not implemented.
            /// </summary>
            /// <param name="context">Not in use.</param>
            public override void Execute(IEventContext context)
            {
                throw new NotImplementedException();
            }
        }
    }
}
