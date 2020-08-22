// -----------------------------------------------------------------
// <copyright file="IScheduler.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Scheduling.Contracts.Abstractions
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Fibula.Scheduling.Contracts.Delegates;

    /// <summary>
    /// Interface that represents a scheduler.
    /// </summary>
    public interface IScheduler
    {
        /// <summary>
        /// Event fired when an event gets fired by the scheduler.
        /// </summary>
        event EventFiredDelegate EventFired;

        /// <summary>
        /// Gets the current time.
        /// </summary>
        DateTimeOffset CurrentTime { get; }

        /// <summary>
        /// Gets the queue size of the scheduler.
        /// </summary>
        int QueueSize { get; }

        /// <summary>
        /// Schedules an event to be fired at the specified time.
        /// </summary>
        /// <param name="eventToSchedule">The event to schedule.</param>
        /// <param name="delayTime">Optional. The time delay after which the event should be fired. If left null, the event is scheduled to be fired ASAP.</param>
        /// <param name="scheduleAsync">Optional. A value indicating whether to schedule asynchronously or not.</param>
        void ScheduleEvent(IEvent eventToSchedule, TimeSpan? delayTime = null, bool scheduleAsync = false);

        /// <summary>
        /// Calculates the time left to fire a given event.
        /// </summary>
        /// <param name="evt">The event to calculate against.</param>
        /// <returns>A <see cref="TimeSpan"/> representing the time left to fire, or <see cref="TimeSpan.Zero"/>.</returns>
        TimeSpan CalculateTimeToFire(IEvent evt);

        /// <summary>
        /// Cancels an event.
        /// </summary>
        /// <param name="evt">The event to cancel.</param>
        /// <returns>True if the event is cancelled, false otherwise.</returns>
        bool CancelEvent(IEvent evt);

        /// <summary>
        /// Cancels all events attributed to the specified requestor.
        /// </summary>
        /// <param name="requestorId">The id of the requestor.</param>
        /// <param name="specificType">Optional. The type of event to remove. By default, it will remove all.</param>
        void CancelAllFor(uint requestorId, Type specificType = null);

        /// <summary>
        /// Begins the scheduler's processing the queue and firing events.
        /// </summary>
        /// <param name="cancellationToken">A token to observe for cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous processing operation.</returns>
        Task RunAsync(CancellationToken cancellationToken);
    }
}
