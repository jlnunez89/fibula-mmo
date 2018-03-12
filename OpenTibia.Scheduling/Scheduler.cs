// <copyright file="Scheduler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Scheduling
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using OpenTibia.Server.Interfaces;
    using OpenTibia.Server.Utils;
    using Priority_Queue;

    /// <summary>
    /// Class that represents a scheduler for events.
    /// </summary>
    public class Scheduler : IScheduler
    {
        /// <summary>
        /// The maximum number of nodes that the internal queue can hold.
        /// </summary>
        private const int MaxQueueNodes = 1000000;

        /// <summary>
        /// The maximum difference in hours that the referenced time can be off on creaation of the <see cref="Scheduler"/> instance.
        /// </summary>
        private const int MaximumReferenceTimeDifferenceInHours = 1;

        /// <summary>
        /// The default processing wait time on the processing queue thread.
        /// </summary>
        private static readonly TimeSpan DefaultProcessWaitTime = TimeSpan.FromMinutes(1);

        /// <summary>
        /// The referenced start time.
        /// </summary>
        private readonly DateTime startTime;

        /// <summary>
        /// The internal priority queue used to manage events.
        /// </summary>
        private FastPriorityQueue<BaseEvent> priorityQueue;

        /// <summary>
        /// A cancellation token to use on the queue processing thread.
        /// </summary>
        private CancellationToken cancellationToken;

        /// <summary>
        /// A lock object to semaphore queue modifications.
        /// </summary>
        private object queueLock;

        /// <summary>
        /// A lock object to monitor when new events are added to the queue.
        /// </summary>
        private object eventsAvailableLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="Scheduler"/> class.
        /// </summary>
        /// <param name="referenceTime">The time to use as reference .</param>
        public Scheduler(DateTime referenceTime)
        {
            referenceTime.ThrowIfDefaultValue(nameof(referenceTime));

            var refTimeDifference = (DateTime.Now - referenceTime).TotalHours;

            if (refTimeDifference >= Scheduler.MaximumReferenceTimeDifferenceInHours)
            {
                throw new ArgumentException($"{nameof(referenceTime)} must be within the past hour's time.");
            }

            this.eventsAvailableLock = new object();
            this.queueLock = new object();
            this.startTime = referenceTime;
            this.priorityQueue = new FastPriorityQueue<BaseEvent>(Scheduler.MaxQueueNodes);
            this.cancellationToken = new CancellationTokenSource().Token;

            Task.Factory.StartNew(this.QueueProcessing, this.cancellationToken);
        }

        /// <inheritdoc/>
        public event EventFired OnEventFired;

        /// <inheritdoc/>
        public void ImmediateEvent(IEvent eventToSchedule)
        {
            eventToSchedule.ThrowIfNull(nameof(eventToSchedule));

            var castedEvent = eventToSchedule as BaseEvent;

            if (castedEvent == null)
            {
                throw new ArgumentException($"Argument must be of type {nameof(BaseEvent)}.", nameof(eventToSchedule));
            }

            lock (this.eventsAvailableLock)
            {
                lock (this.queueLock)
                {
                    this.priorityQueue.Enqueue(castedEvent, 0);
                }

                Monitor.Pulse(this.eventsAvailableLock);
            }
        }

        /// <inheritdoc/>
        public void ScheduleEvent(IEvent eventToSchedule, DateTime runAt)
        {
            eventToSchedule.ThrowIfNull(nameof(eventToSchedule));

            var castedEvent = eventToSchedule as BaseEvent;

            if (castedEvent == null)
            {
                throw new ArgumentException($"Argument must be of type {nameof(BaseEvent)}.", nameof(eventToSchedule));
            }

            runAt.ThrowIfDefaultValue(nameof(runAt));

            if (runAt < this.startTime)
            {
                throw new ArgumentException($"Value cannot be earlier than the reference time of the scheduler: {this.startTime}.", nameof(runAt));
            }

            lock (this.eventsAvailableLock)
            {
                lock (this.queueLock)
                {
                    this.priorityQueue.Enqueue(castedEvent, this.GetMillisecondsAfterReferenceTime(runAt));
                }

                Monitor.Pulse(this.eventsAvailableLock);
            }
        }

        /// <summary>
        /// Calculates the total millisenconds value of the time difference between the specified time and when the scheduler began.
        /// </summary>
        /// <param name="dateTime">The specified time.</param>
        /// <returns>The milliseconds value.</returns>
        private long GetMillisecondsAfterReferenceTime(DateTime dateTime)
        {
            return Convert.ToInt64((dateTime - this.startTime).TotalMilliseconds);
        }

        /// <summary>
        /// Processes the queue and fires events.
        /// </summary>
        private void QueueProcessing()
        {
            TimeSpan waitForNewTimeOut = TimeSpan.Zero;

            while (!this.cancellationToken.IsCancellationRequested)
            {
                lock (this.eventsAvailableLock)
                {
                    // wait until we're flagged that there are events available.
                    Monitor.Wait(this.eventsAvailableLock, waitForNewTimeOut > TimeSpan.Zero ? waitForNewTimeOut : Scheduler.DefaultProcessWaitTime);

                    // reset time to wait.
                    waitForNewTimeOut = TimeSpan.Zero;

                    lock (this.queueLock)
                    {
                        if (this.priorityQueue.First == null)
                        {
                            // no more items on the queue, go to wait.
                            continue;
                        }

                        // store a single 'current' time for processing of all items in the queue
                        // TODO: use 'current' time from Game.Instance
                        var currentTimeInMilliseconds = this.GetMillisecondsAfterReferenceTime(DateTime.Now);

                        // check the current queue and fire any events that are due.
                        while (this.priorityQueue.Count > 0)
                        {
                            // the first item always points to the next-in-time event available.
                            var nextEvent = this.priorityQueue.First;

                            // check if the event is due
                            if (nextEvent.Priority <= currentTimeInMilliseconds)
                            {
                                // actually dequeue this item.
                                this.priorityQueue.Dequeue();

                                this.OnEventFired?.Invoke(this, new EventFiredEventArgs(nextEvent));
                                continue;
                            }

                            // else the next item is in the future, so figure out how long to wait, update and break.
                            waitForNewTimeOut = TimeSpan.FromMilliseconds(nextEvent.Priority - currentTimeInMilliseconds);
                            break;
                        }
                    }
                }
            }
        }
    }
}
