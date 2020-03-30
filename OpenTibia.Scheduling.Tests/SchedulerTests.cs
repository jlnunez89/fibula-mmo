// -----------------------------------------------------------------
// <copyright file="SchedulerTests.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Scheduling.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using OpenTibia.Common.Utilities.Testing;
    using OpenTibia.Scheduling.Contracts.Abstractions;
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

            // use a non default reference time.
            new Scheduler(loggerMock.Object);
        }

        /// <summary>
        /// Checks that <see cref="Scheduler.ScheduleEvent(IEvent, TimeSpan?)"/> throws when needed.
        /// </summary>
        [TestMethod]
        public void ScheduleEvent_Throws_WhenBad()
        {
            const uint RequestorId = 0;

            Mock<ILogger> loggerMock = new Mock<ILogger>();
            Mock<ILogger> schedulerLoggerMock = new Mock<ILogger>();

            schedulerLoggerMock.Setup(l => l.ForContext<Scheduler>()).Returns(schedulerLoggerMock.Object);

            TimeSpan oneMillisecondDelayTime = TimeSpan.FromMilliseconds(1);
            TimeSpan twoSecondsDelayTime = TimeSpan.FromSeconds(2);

            Scheduler scheduler = new Scheduler(schedulerLoggerMock.Object);

            ExceptionAssert.Throws<ArgumentNullException>(() => scheduler.ScheduleEvent(null, oneMillisecondDelayTime), $"Value cannot be null. (Parameter 'eventToSchedule')");

            Mock<IEvent> eventMock = new Mock<IEvent>();

            ExceptionAssert.Throws<ArgumentException>(() => scheduler.ScheduleEvent(eventMock.Object, oneMillisecondDelayTime), $"Argument must be of type {nameof(BaseEvent)}. (Parameter 'eventToSchedule')");

            Mock<BaseEvent> bEventMock = new Mock<BaseEvent>(loggerMock.Object, RequestorId);

            // schedule twice
            scheduler.ScheduleEvent(bEventMock.Object, twoSecondsDelayTime);

            ExceptionAssert.Throws<ArgumentException>(() => scheduler.ScheduleEvent(bEventMock.Object), $"The event is already scheduled. (Parameter 'eventToSchedule')");
        }

        /// <summary>
        /// Checks that <see cref="Scheduler.CancelEvent(string)"/> does what it should.
        /// </summary>
        [TestMethod]
        public void Cancelling_SingleEvent()
        {
            const uint RequestorId = 0;

            Mock<ILogger> loggerMock = new Mock<ILogger>();
            Mock<ILogger> schedulerLoggerMock = new Mock<ILogger>();

            schedulerLoggerMock.Setup(l => l.ForContext<Scheduler>()).Returns(schedulerLoggerMock.Object);

            TimeSpan overheadDelay = TimeSpan.FromMilliseconds(100);
            TimeSpan twoSecondsTimeSpan = TimeSpan.FromSeconds(2);
            TimeSpan threeSecondsTimeSpan = TimeSpan.FromSeconds(3);

            const int ExpectedCounterValueBeforeRun = 0;
            const int ExpectedCounterValueAfterRun = 0;

            var scheduledEventFiredCounter = 0;

            Mock<BaseEvent> bEventMockForScheduled = new Mock<BaseEvent>(loggerMock.Object, RequestorId);

            Scheduler scheduler = new Scheduler(schedulerLoggerMock.Object);

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

            // fire a scheduled event that shall be fired only after some seconds.
            scheduler.ScheduleEvent(bEventMockForScheduled.Object, twoSecondsTimeSpan);

            // delay for 100 ms (to account for setup overhead and multi threading) and check that the counter has NOT gone up for scheduled
            Task.Delay(overheadDelay)
                .ContinueWith(prev =>
                {
                    Assert.AreEqual(ExpectedCounterValueBeforeRun, scheduledEventFiredCounter, $"Scheduled events counter does not match: Expected {ExpectedCounterValueBeforeRun}, got {scheduledEventFiredCounter}.");
                })
                .Wait();

            // cancel this event.
            scheduler.CancelEvent(bEventMockForScheduled.Object.EventId);

            // delay for three seconds and check that the counter has NOT gone up for scheduled.
            Task.Delay(threeSecondsTimeSpan)
                .ContinueWith(prev =>
                {
                    Assert.AreEqual(ExpectedCounterValueAfterRun, scheduledEventFiredCounter, $"Scheduled events counter does not match: Expected {ExpectedCounterValueAfterRun}, got {scheduledEventFiredCounter}.");
                })
                .Wait();
        }

        /// <summary>
        /// Checks that <see cref="Scheduler.CancelAllFor"/> does what it should.
        /// </summary>
        [TestMethod]
        public void Cancelling_AllEventsFor()
        {
            Mock<ILogger> loggerMock = new Mock<ILogger>();
            Mock<ILogger> schedulerLoggerMock = new Mock<ILogger>();

            schedulerLoggerMock.Setup(l => l.ForContext<Scheduler>()).Returns(schedulerLoggerMock.Object);

            TimeSpan overheadDelay = TimeSpan.FromMilliseconds(100);
            TimeSpan twoSecondsTimeSpan = TimeSpan.FromSeconds(2);
            TimeSpan threeSecondsTimeSpan = TimeSpan.FromSeconds(3);

            const uint anyRequestorId = 100u;
            const int ExpectedCounterValueBeforeRun = 0;
            const int ExpectedCounterValueAfterRun = 0;

            var scheduledEventFiredCounter = 0;

            Mock<BaseEvent> bEventMockForScheduled1 = new Mock<BaseEvent>(loggerMock.Object, anyRequestorId);
            Mock<BaseEvent> bEventMockForScheduled2 = new Mock<BaseEvent>(loggerMock.Object, anyRequestorId);
            Mock<BaseEvent> bEventMockForScheduled3 = new Mock<BaseEvent>(loggerMock.Object, anyRequestorId);

            Scheduler scheduler = new Scheduler(schedulerLoggerMock.Object);

            scheduler.EventFired += (sender, eventArgs) =>
            {
                // test that sender is the same scheduler instance, while we're here.
                Assert.AreEqual(scheduler, sender);

                // check that event has a reference.
                Assert.IsNotNull(eventArgs?.Event);

                scheduledEventFiredCounter++;
            };

            // fire a scheduled event that shall be fired only after some seconds.
            scheduler.ScheduleEvent(bEventMockForScheduled1.Object, twoSecondsTimeSpan);
            scheduler.ScheduleEvent(bEventMockForScheduled2.Object, twoSecondsTimeSpan);
            scheduler.ScheduleEvent(bEventMockForScheduled3.Object, twoSecondsTimeSpan);

            // delay for 100 ms (to account for setup overhead and multi threading) and check that the counter has NOT gone up for scheduled
            Task.Delay(overheadDelay)
                .ContinueWith(prev =>
                {
                    Assert.AreEqual(ExpectedCounterValueBeforeRun, scheduledEventFiredCounter, $"Scheduled events counter does not match: Expected {ExpectedCounterValueBeforeRun}, got {scheduledEventFiredCounter}.");
                })
                .Wait();

            // cancel this event.
            scheduler.CancelAllFor(anyRequestorId);

            // delay for three seconds and check that the counter has NOT gone up for scheduled.
            Task.Delay(threeSecondsTimeSpan)
                .ContinueWith(prev =>
                {
                    Assert.AreEqual(ExpectedCounterValueAfterRun, scheduledEventFiredCounter, $"Scheduled events counter does not match: Expected {ExpectedCounterValueAfterRun}, got {scheduledEventFiredCounter}.");
                })
                .Wait();
        }

        /// <summary>
        /// Checks that <see cref="Scheduler.EventFired"/> gets fired when an event is scheduled.
        /// </summary>
        [TestMethod]
        public void OnEventFired_IsCalled()
        {
            const uint RequestorId = 0;

            Mock<ILogger> loggerMock = new Mock<ILogger>();
            Mock<ILogger> schedulerLoggerMock = new Mock<ILogger>();

            schedulerLoggerMock.Setup(l => l.ForContext<Scheduler>()).Returns(schedulerLoggerMock.Object);

            const int ExpectedCounterValueBeforeRun = 0;
            const int ExpectedCounterValueAfterRun = 1;

            TimeSpan twoSecondsTimeSpan = TimeSpan.FromSeconds(2);
            TimeSpan overheadDelay = TimeSpan.FromMilliseconds(100);

            Mock<BaseEvent> bEventMockForInmediate = new Mock<BaseEvent>(loggerMock.Object, RequestorId);
            Mock<BaseEvent> bEventMockForScheduled = new Mock<BaseEvent>(loggerMock.Object, RequestorId);

            Scheduler scheduler = new Scheduler(schedulerLoggerMock.Object);
            var inmediateEventFiredCounter = 0;
            var scheduledEventFiredCounter = 0;

            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
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
            }

            // fire a scheduled event that shall be fired only after some seconds.
            scheduler.ScheduleEvent(bEventMockForScheduled.Object, twoSecondsTimeSpan);

            // delay for 100 ms (to account for setup overhead and multi threading) and check that the counter has NOT gone up for scheduled
            Task.Delay(overheadDelay)
                .ContinueWith(prev =>
                    {
                        Assert.AreEqual(ExpectedCounterValueBeforeRun, scheduledEventFiredCounter, $"Scheduled events counter does not match: Expected {ExpectedCounterValueBeforeRun}, got {scheduledEventFiredCounter}.");
                    })
                .Wait();

            // fire the inmediate event, which should be run asap.
            scheduler.ScheduleEvent(bEventMockForInmediate.Object);

            // delay for 500 ms and check that the counter has gone up.
            Task.Delay(overheadDelay)
                .ContinueWith(prev =>
                    {
                        Assert.AreEqual(ExpectedCounterValueAfterRun, inmediateEventFiredCounter, $"Inmediate events counter does not match: Expected {ExpectedCounterValueAfterRun}, got {inmediateEventFiredCounter}.");
                    })
                .Wait();

            // delay for the remaining seconds and check that the counter has gone up for scheduled.
            Task.Delay(twoSecondsTimeSpan)
                .ContinueWith(prev =>
                    {
                        Assert.AreEqual(ExpectedCounterValueAfterRun, scheduledEventFiredCounter, $"Scheduled events counter does not match: Expected {ExpectedCounterValueAfterRun}, got {scheduledEventFiredCounter}.");
                    })
                .Wait();
        }
    }
}
