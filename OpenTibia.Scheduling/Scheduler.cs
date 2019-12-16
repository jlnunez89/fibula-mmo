// -----------------------------------------------------------------
// <copyright file="Scheduler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Scheduling
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Scheduling.Contracts;
    using OpenTibia.Scheduling.Contracts.Abstractions;
    using Priority_Queue;
    using Serilog;

    /// <summary>
    /// Class that represents a scheduler for events.
    /// </summary>
    public class Scheduler : IScheduler
    {
        /// <summary>
        /// The maximum number of nodes that the internal queue can hold.
        /// </summary>
        /// <remarks>Arbitrarily chosen, resize as needed.</remarks>
        private const int MaxQueueNodes = 1000000;

        /// <summary>
        /// The default processing wait time on the processing queue thread.
        /// </summary>
        private static readonly TimeSpan DefaultProcessWaitTime = TimeSpan.FromMinutes(1);

        /// <summary>
        /// The start time of the scheduler.
        /// </summary>
        private readonly DateTimeOffset startTime;

        /// <summary>
        /// The internal priority queue used to manage events.
        /// </summary>
        private readonly FastPriorityQueue<BaseEvent> priorityQueue;

        /// <summary>
        /// Stores the ids of cancelled events.
        /// </summary>
        private readonly ISet<string> cancelledEvents;

        /// <summary>
        /// A dictionary to keep track of who requested which events.
        /// </summary>
        private readonly IDictionary<uint, ISet<string>> eventsPerRequestor;

        /// <summary>
        /// A lock object to semaphore queue modifications.
        /// </summary>
        private readonly object queueLock;

        /// <summary>
        /// A lock object to monitor when new events are added to the queue.
        /// </summary>
        private readonly object eventsAvailableLock;

        /// <summary>
        /// A lock object to semaphore the events per requestor dictionary.
        /// </summary>
        private readonly object eventsPerRequestorLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="Scheduler"/> class.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        public Scheduler(ILogger logger)
        {
            logger.ThrowIfNull(nameof(logger));

            this.Logger = logger.ForContext<Scheduler>();

            this.eventsPerRequestorLock = new object();
            this.eventsAvailableLock = new object();
            this.queueLock = new object();
            this.startTime = this.CurrentTime;
            this.priorityQueue = new FastPriorityQueue<BaseEvent>(MaxQueueNodes);
            this.cancelledEvents = new HashSet<string>();
            this.eventsPerRequestor = new Dictionary<uint, ISet<string>>();
        }

        /// <summary>
        /// Event fired when an event gets fired by the scheduler.
        /// </summary>
        public event EventFired OnEventFired;

        /// <summary>
        /// Gets a reference to the logger instance.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Gets the current time.
        /// </summary>
        public DateTimeOffset CurrentTime => DateTimeOffset.UtcNow;

        /// <summary>
        /// Begins the scheduler's processing the queue and firing events.
        /// </summary>
        /// <param name="cancellationToken">A token to observe for cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous processing operation.</returns>
        public Task RunAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                this.Logger.Debug("Scheduler started.");

                TimeSpan waitForNewTimeOut = TimeSpan.Zero;

                while (!cancellationToken.IsCancellationRequested)
                {
                    lock (this.eventsAvailableLock)
                    {
                        // wait until we're flagged that there are events available.
                        Monitor.Wait(this.eventsAvailableLock, waitForNewTimeOut > TimeSpan.Zero ? waitForNewTimeOut : DefaultProcessWaitTime);

                        // reset time to wait.
                        waitForNewTimeOut = TimeSpan.Zero;

                        lock (this.queueLock)
                        {
                            if (this.priorityQueue.First == null)
                            {
                                // no more items on the queue, go to wait.
                                this.Logger.Warning("Queue empty.");
                                continue;
                            }

                            var currentTimeInMilliseconds = this.GetMillisecondsAfterReferenceTime(this.CurrentTime);

                            // check the current queue and fire any events that are due.
                            while (this.priorityQueue.Count > 0)
                            {
                                // the first item always points to the next-in-time event available.
                                var nextEvent = this.priorityQueue.First;

                                // check if this event has been cancelled.
                                if (this.cancelledEvents.Contains(nextEvent.EventId))
                                {
                                    // dequeue, clean and move next.
                                    this.priorityQueue.Dequeue();
                                    this.CleanAllAttributedTo(nextEvent.EventId);
                                    continue;
                                }

                                // check if the event is due
                                if (nextEvent.Priority <= currentTimeInMilliseconds)
                                {
                                    // actually dequeue this item.
                                    this.priorityQueue.Dequeue();

                                    this.OnEventFired?.Invoke(this, new EventFiredEventArgs(nextEvent));
                                    continue;
                                }

                                // else the next item is in the future, so figure out how long to wait, update and break.
                                waitForNewTimeOut = TimeSpan.FromMilliseconds(nextEvent.Priority < currentTimeInMilliseconds ? 0 : nextEvent.Priority - currentTimeInMilliseconds);
                                break;
                            }
                        }
                    }
                }

                this.Logger.Debug("Scheduler finished.");
            });
        }

        /// <summary>
        /// Cancels all the events attributed to a requestor.
        /// </summary>
        /// <param name="requestorId">The id of the requestor.</param>
        /// <param name="specificType">Optional. The type of event to remove. By default, it will remove all.</param>
        public void CancelAllFor(uint requestorId, Type specificType = null)
        {
            requestorId.ThrowIfDefaultValue();

            if (specificType == null)
            {
                specificType = typeof(BaseEvent);
            }

            if (!typeof(IEvent).IsAssignableFrom(specificType))
            {
                throw new ArgumentException($"Invalid type of event specified. Type must derive from {nameof(IEvent)}.", nameof(specificType));
            }

            lock (this.eventsPerRequestorLock)
            {
                if (!this.eventsPerRequestor.ContainsKey(requestorId) || this.eventsPerRequestor[requestorId].Count == 0)
                {
                    return;
                }

                foreach (var eventId in this.eventsPerRequestor[requestorId])
                {
                    // if (specificType == null || !eventTypes[eventId].IsAssignableFrom(specificType))
                    this.CancelEvent(eventId);
                }

                this.eventsPerRequestor.Remove(requestorId);
            }
        }

        /// <summary>
        /// Cancels an event.
        /// </summary>
        /// <param name="eventId">The id of the event to cancel.</param>
        public void CancelEvent(string eventId)
        {
            eventId.ThrowIfNullOrWhiteSpace();

            try
            {
                this.cancelledEvents.Add(eventId);
            }
            catch
            {
                // just ignore any collisions.
            }
        }

        /// <inheritdoc/>
        public void ImmediateEvent(IEvent eventToSchedule)
        {
            eventToSchedule.ThrowIfNull(nameof(eventToSchedule));

            if (!(eventToSchedule is BaseEvent castedEvent))
            {
                throw new ArgumentException($"Argument must be of type {nameof(BaseEvent)}.", nameof(eventToSchedule));
            }

            lock (this.eventsAvailableLock)
            {
                lock (this.queueLock)
                {
                    if (this.priorityQueue.Contains(castedEvent))
                    {
                        throw new ArgumentException($"The event is already scheduled.", nameof(eventToSchedule));
                    }

                    this.priorityQueue.Enqueue(castedEvent, 0);
                }

                // check and add event attribution to the requestor
                if (castedEvent.RequestorId > 0)
                {
                    lock (this.eventsPerRequestorLock)
                    {
                        if (!this.eventsPerRequestor.ContainsKey(castedEvent.RequestorId))
                        {
                            this.eventsPerRequestor.Add(castedEvent.RequestorId, new HashSet<string>());
                        }

                        this.eventsPerRequestor[castedEvent.RequestorId].Add(castedEvent.EventId);
                    }
                }

                Monitor.Pulse(this.eventsAvailableLock);
            }
        }

        /// <inheritdoc/>
        public void ScheduleEvent(IEvent eventToSchedule, DateTimeOffset runAt)
        {
            eventToSchedule.ThrowIfNull(nameof(eventToSchedule));

            if (!(eventToSchedule is BaseEvent castedEvent))
            {
                throw new ArgumentException($"Argument must be of type {nameof(BaseEvent)}.", nameof(eventToSchedule));
            }

            if (runAt < this.startTime)
            {
                throw new ArgumentException($"Value cannot be earlier than the reference time of the scheduler: {this.startTime}.", nameof(runAt));
            }

            lock (this.eventsAvailableLock)
            {
                lock (this.queueLock)
                {
                    if (this.priorityQueue.Contains(castedEvent))
                    {
                        throw new ArgumentException($"The event is already scheduled.", nameof(eventToSchedule));
                    }

                    this.priorityQueue.Enqueue(castedEvent, this.GetMillisecondsAfterReferenceTime(runAt));
                }

                // check and add event attribution to the requestor
                if (castedEvent.RequestorId > 0)
                {
                    lock (this.eventsPerRequestorLock)
                    {
                        if (!this.eventsPerRequestor.ContainsKey(castedEvent.RequestorId))
                        {
                            this.eventsPerRequestor.Add(castedEvent.RequestorId, new HashSet<string>());
                        }

                        this.eventsPerRequestor[castedEvent.RequestorId].Add(castedEvent.EventId);
                    }
                }

                Monitor.Pulse(this.eventsAvailableLock);
            }
        }

        /// <summary>
        /// Calculates the total millisenconds value of the time difference between the specified time and when the scheduler began.
        /// </summary>
        /// <param name="dateTime">The specified time.</param>
        /// <returns>The milliseconds value.</returns>
        private long GetMillisecondsAfterReferenceTime(DateTimeOffset dateTime)
        {
            return Convert.ToInt64((dateTime - this.startTime).TotalMilliseconds);
        }

        /// <summary>
        /// Cleans all events attributed to the specified event or requestor id.
        /// </summary>
        /// <param name="eventId">The id of the event to cancel.</param>
        /// <param name="eventRequestor">The id of the requestor.</param>
        private void CleanAllAttributedTo(string eventId, uint eventRequestor = 0)
        {
            try
            {
                this.cancelledEvents.Remove(eventId);
            }
            catch
            {
                // ignore, as if this fails then the value is not there.
            }

            if (eventRequestor == 0)
            {
                // no requestor, so it shouldn't be on the other dictionary.
                return;
            }

            try
            {
                lock (this.eventsPerRequestorLock)
                {
                    this.eventsPerRequestor[eventRequestor].Remove(eventId);

                    if (this.eventsPerRequestor[eventRequestor].Count == 0)
                    {
                        this.eventsPerRequestor.Remove(eventRequestor);
                    }
                }
            }
            catch
            {
                // ignore, as if this fails then the value is not there.
            }
        }
    }
}
