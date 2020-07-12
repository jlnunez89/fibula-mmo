// -----------------------------------------------------------------
// <copyright file="INotification.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Contracts.Abstractions
{
    using Fibula.Mechanics.Contracts.Delegates;
    using Fibula.Scheduling.Contracts.Abstractions;

    /// <summary>
    /// Interface for all notifications.
    /// </summary>
    public interface INotification : IEvent
    {
        /// <summary>
        /// Event to call when the notification is sent.
        /// </summary>
        event OnSent Sent;

        /// <summary>
        /// Sends the notification to the players intented.
        /// </summary>
        /// <param name="context">The context for this notification.</param>
        void Send(INotificationContext context);
    }
}
