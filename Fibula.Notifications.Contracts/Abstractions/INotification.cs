// -----------------------------------------------------------------
// <copyright file="INotification.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Notifications.Contracts.Abstractions
{
    using Fibula.Notifications.Contracts.Delegates;
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
