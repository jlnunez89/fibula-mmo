// -----------------------------------------------------------------
// <copyright file="Scheduler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Scheduling
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Fibula.Common.Utilities;
    using Fibula.Scheduling.Contracts.Abstractions;
    using Fibula.Scheduling.Contracts.Delegates;
    using Priority_Queue;
    using Serilog;

    /// <summary>
    /// Class that represents a scheduler for events.
    /// </summary>
    public class Scheduler : IScheduler
    {
        /// <summary>
        /// The default processing wait time on the processing queue thread.
        /// </summary>
        private static readonly TimeSpan DefaultProcessWaitTime = TimeSpan.FromSeconds(5);

        /// <summary>
        /// The start time of the scheduler.
        /// </summary>
        private readonly DateTimeOffset startTime;

        /// <summary>
        /// The internal priority queue used to manage events.
        /// </summary>
        private readonly StablePriorityQueue<BaseEvent> priorityQueue;

        /// <summary>
        /// Stores the ids of cancelled events.
        /// </summary>
        private readonly ISet<string> cancelledEvents;

        /// <summary>
        /// A dictionary to keep track of who requested which events.
        /// </summary>
        private readonly IDictionary<uint, ISet<string>> eventsPerRequestor;

        /// <summary>
        /// A dictionary to keep track of the type of events.
        /// </summary>
        private readonly IDictionary<string, Type> eventTypes;

        /// <summary>
        /// A lock object to monitor when new events are added to the queue.
        /// </summary>
        private readonly object eventsAvailableLock;

        /// <summary>
        /// A lock object to semaphore the events per requestor dictionary.
        /// </summary>
        private readonly object eventsPerRequestorLock;

        /// <summary>
        /// The maximum number of nodes that the internal queue can hold.
        /// </summary>
        /// <remarks>Arbitrarily chosen, resize happens as needed doubling each time, but also costs double time and space each time.</remarks>
        private int maxQueueNodes = 256;

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
            this.startTime = this.CurrentTime;
            this.priorityQueue = new StablePriorityQueue<BaseEvent>(this.maxQueueNodes);
            this.cancelledEvents = new HashSet<string>();
            this.eventsPerRequestor = new Dictionary<uint, ISet<string>>();
            this.eventTypes = new Dictionary<string, Type>();
        }

        /// <summary>
        /// When an event gets fired by the this scheduler.
        /// </summary>
        public event EventFiredDelegate EventFired;

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
                Stopwatch sw = new Stopwatch();

                long cyclesProcessed = 0;
                long cycleTimeTotal = 0;

                while (!cancellationToken.IsCancellationRequested)
                {
                    lock (this.eventsAvailableLock)
                    {
                        // Wait until we're flagged that there are events available.
                        // Note that we normalize waitForNewTimeOut to be between Zero and DefaultProcessWaitTime)
                        if (waitForNewTimeOut < TimeSpan.Zero)
                        {
                            // Normalize to zero because the Monitor.Wait() call throws on negative values.
                            waitForNewTimeOut = TimeSpan.Zero;
                        }
                        else if (waitForNewTimeOut > DefaultProcessWaitTime)
                        {
                            waitForNewTimeOut = DefaultProcessWaitTime;
                        }

                        Monitor.Wait(this.eventsAvailableLock, waitForNewTimeOut);

                        // Then, we reset time to wait and start the stopwatch.
                        waitForNewTimeOut = DefaultProcessWaitTime;
                        sw.Restart();

                        if (this.priorityQueue.First == null)
                        {
                            // no more items on the queue, go to wait.
                            this.Logger.Warning("Queue empty.");
                            continue;
                        }

                        var processTimeAdjustment = cyclesProcessed == 0 ? 0 : cycleTimeTotal / cyclesProcessed++;

                        var priorityDue = this.GetMillisecondsAfterReferenceTime(this.CurrentTime) - processTimeAdjustment;

                        // Check the current queue and fire any events that are due.
                        while (this.priorityQueue.Count > 0)
                        {
                            if (this.priorityQueue.Count > (int)(this.maxQueueNodes * 0.80))
                            {
                                this.Logger.Warning($"Queue is over 80% capacity, doubling the size of the queue before we run out...");

                                // double the max queue size and resize it.
                                this.maxQueueNodes *= 2;
                                this.priorityQueue.Resize(this.maxQueueNodes);

                                this.Logger.Warning($"Resized queue max size to {this.maxQueueNodes}.");
                            }

                            // The first item always points to the next-in-time event available.
                            var evt = this.priorityQueue.First;
                            var isDue = evt.Priority <= priorityDue;
                            var shouldBeSkipped = this.cancelledEvents.Contains(evt.EventId);

                            // Check if this event has been cancelled or is due.
                            if (isDue || shouldBeSkipped)
                            {
                                // Actually dequeue the event.
                                this.priorityQueue.Dequeue();

                                if (!shouldBeSkipped)
                                {
                                    this.Logger.Verbose($"Firing {evt.GetType().Name} with id {evt.EventId}, at {priorityDue}.");

                                    this.EventFired?.Invoke(this, new EventFiredEventArgs(evt));
                                }

                                // Clean the processed event.
                                this.CleanUp(evt.EventId, evt.RequestorId);

                                // And clean up the expedition hook.
                                evt.Expedited -= this.HandleEventExpedition;
                            }
                            else
                            {
                                // The next item is in the future, so figure out how long to wait, update and break.
                                waitForNewTimeOut = TimeSpan.FromMilliseconds(evt.Priority < priorityDue ? 0 : evt.Priority - priorityDue);
                                break;
                            }
                        }

                        sw.Stop();

                        cycleTimeTotal += sw.ElapsedMilliseconds;
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

            lock (this.eventsAvailableLock)
            {
                lock (this.eventsPerRequestorLock)
                {
                    if (!this.eventsPerRequestor.ContainsKey(requestorId) || this.eventsPerRequestor[requestorId].Count == 0)
                    {
                        return;
                    }

                    foreach (var eventId in this.eventsPerRequestor[requestorId])
                    {
                        if (!this.eventTypes.TryGetValue(eventId, out Type evetType) || specificType.IsAssignableFrom(evetType))
                        {
                            this.CancelEvent(eventId);

                            this.Logger.Verbose($"Cancelled {specificType.Name} with id {eventId}.");
                        }
                    }

                    this.eventsPerRequestor.Remove(requestorId);
                }
            }
        }

        /// <summary>
        /// Cancels an event.
        /// </summary>
        /// <param name="eventId">The id of the event to cancel.</param>
        public void CancelEvent(string eventId)
        {
            eventId.ThrowIfNullOrWhiteSpace();

            // Lock on the events available to prevent race conditions on cancel vs firing.
            lock (this.eventsAvailableLock)
            {
                this.cancelledEvents.Add(eventId);
            }
        }

        /// <summary>
        /// Schedules an event to be fired after the specified delay time.
        /// </summary>
        /// <param name="eventToSchedule">The event to schedule.</param>
        /// <param name="delayTime">Optional. The time delay after which the event should be fired. If left null, the event is scheduled to be fired ASAP.</param>
        public void ScheduleEvent(IEvent eventToSchedule, TimeSpan? delayTime = null)
        {
            eventToSchedule.ThrowIfNull(nameof(eventToSchedule));

            if (!(eventToSchedule is BaseEvent castedEvent))
            {
                throw new ArgumentException($"Argument must be of type {nameof(BaseEvent)}.", nameof(eventToSchedule));
            }

            if (delayTime == null || delayTime < TimeSpan.Zero)
            {
                delayTime = TimeSpan.Zero;
            }

            lock (this.eventsAvailableLock)
            {
                if (this.priorityQueue.Contains(castedEvent))
                {
                    throw new ArgumentException($"The event is already scheduled.", nameof(eventToSchedule));
                }

                var targetTime = this.CurrentTime + delayTime.Value;
                var milliseconds = this.GetMillisecondsAfterReferenceTime(targetTime);

                if (!castedEvent.HasExpeditionHandler)
                {
                    castedEvent.Expedited += this.HandleEventExpedition;
                }

                this.priorityQueue.Enqueue(castedEvent, milliseconds);

                this.Logger.Verbose($"Scheduled {eventToSchedule.GetType().Name} with id {eventToSchedule.EventId}, due in {delayTime.Value} (at {targetTime.ToUnixTimeMilliseconds()}).");

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

                        if (!this.eventTypes.ContainsKey(castedEvent.EventId))
                        {
                            this.eventTypes[castedEvent.EventId] = eventToSchedule.GetType();
                        }
                    }
                }

                Monitor.Pulse(this.eventsAvailableLock);
            }
        }

        /// <summary>
        /// Handles a call from an expedited event.
        /// </summary>
        /// <param name="sender">The event that was expedited.</param>
        private bool HandleEventExpedition(IEvent sender)
        {
            if (sender == null || !(sender is BaseEvent evt))
            {
                return false;
            }

            // Lock on the events available to prevent race conditions on the manually firing list vs firing.
            lock (this.eventsAvailableLock)
            {
                // Expedite the event by either updating it's priority (if it's enqueued), or enqueueing it without delay.
                if (this.priorityQueue.Contains(evt))
                {
                    this.priorityQueue.UpdatePriority(evt, this.GetMillisecondsAfterReferenceTime(this.CurrentTime));
                }
                else
                {
                    this.ScheduleEvent(evt);
                }

                // Flag the processing queue.
                Monitor.Pulse(this.eventsAvailableLock);
            }

            return true;
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
        private void CleanUp(string eventId, uint eventRequestor = 0)
        {
            this.cancelledEvents.Remove(eventId);

            this.eventTypes.Remove(eventId);

            if (eventRequestor == 0)
            {
                // no requestor, so it shouldn't be on the other dictionary.
                return;
            }

            lock (this.eventsPerRequestorLock)
            {
                if (!this.eventsPerRequestor.ContainsKey(eventRequestor))
                {
                    return;
                }

                this.eventsPerRequestor[eventRequestor].Remove(eventId);

                if (this.eventsPerRequestor[eventRequestor].Count == 0)
                {
                    this.eventsPerRequestor.Remove(eventRequestor);
                }
            }
        }
    }
}
