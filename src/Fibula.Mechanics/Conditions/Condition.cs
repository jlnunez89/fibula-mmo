// -----------------------------------------------------------------
// <copyright file="Condition.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Conditions
{
    using System;
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Utilities;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Mechanics.Notifications;
    using Fibula.Scheduling;
    using Fibula.Scheduling.Contracts.Abstractions;

    /// <summary>
    /// Abstract class that represents a base for all conditions.
    /// </summary>
    public abstract class Condition : BaseEvent, ICondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Condition"/> class.
        /// </summary>
        /// <param name="conditionType">The type of exhaustion.</param>
        public Condition(ConditionType conditionType)
        {
            this.Type = conditionType;

            this.CanBeCancelled = true;
        }

        /// <summary>
        /// Gets a string representing this condition's type.
        /// </summary>
        public override string EventType => this.Type.ToString();

        /// <summary>
        /// Gets the type of this condition.
        /// </summary>
        public ConditionType Type { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this condition can be cured.
        /// </summary>
        public override bool CanBeCancelled { get; protected set; }

        /// <summary>
        /// Executes the event logic.
        /// </summary>
        /// <param name="context">The execution context.</param>
        public override void Execute(IEventContext context)
        {
            context.ThrowIfNull(nameof(context));

            if (!typeof(IConditionContext).IsAssignableFrom(context.GetType()) || !(context is IConditionContext conditionContext))
            {
                throw new ArgumentException($"{nameof(context)} must be an {nameof(IConditionContext)}.");
            }

            // Reset the condition's Repeat property, to avoid implementations running perpetually.
            // It's the responsability of the implementation to extend or repeat duration by modifiying
            // this property each time the condition executes.
            this.RepeatAfter = TimeSpan.MinValue;

            // And execute as condition.
            this.Execute(conditionContext);
        }

        /// <summary>
        /// Aggregates this condition's properties with another of the same type.
        /// </summary>
        /// <param name="conditionOfSameType">The condition to aggregate with.</param>
        /// <returns>True if this condition's properties were changed as a result, and false if nothing changed.</returns>
        public abstract bool Aggregate(ICondition conditionOfSameType);

        /// <summary>
        /// Executes the condition's logic.
        /// </summary>
        /// <param name="context">The execution context for this condition.</param>
        protected abstract void Execute(IConditionContext context);

        /// <summary>
        /// Sends a notification synchronously.
        /// </summary>
        /// <param name="context">A reference to the condition context.</param>
        /// <param name="notification">The notification to send.</param>
        protected void SendNotification(IConditionContext context, INotification notification)
        {
            context.ThrowIfNull(nameof(context));
            notification.ThrowIfNull(nameof(notification));

            notification.Send(new NotificationContext(context.Logger, context.MapDescriptor, context.CreatureFinder));
        }

        /// <summary>
        /// Sends a notification asynchronously.
        /// </summary>
        /// <param name="context">A reference to the condition context.</param>
        /// <param name="notification">The notification to send.</param>
        /// <param name="delayTime">Optional. The time delay after which the notification should be sent. If left null, the notificaion is scheduled to be sent ASAP.</param>
        protected void SendNotificationAsync(IConditionContext context, INotification notification, TimeSpan? delayTime = null)
        {
            context.ThrowIfNull(nameof(context));
            notification.ThrowIfNull(nameof(notification));

            context.Scheduler.ScheduleEvent(notification, delayTime);
        }
    }
}
