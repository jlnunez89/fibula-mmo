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
    using Fibula.Scheduling.Contracts.Abstractions;

    /// <summary>
    /// Interface for all notifications.
    /// </summary>
    public interface INotification : IEvent
    {
        /// <summary>
        /// Sends the notification to the players intented.
        /// </summary>
        /// <param name="context">The context for this notification.</param>
        void Send(INotificationContext context);
    }
}
