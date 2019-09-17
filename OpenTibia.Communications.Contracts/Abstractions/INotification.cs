// -----------------------------------------------------------------
// <copyright file="INotification.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Contracts.Abstractions
{
    using System.Collections.Generic;
    using OpenTibia.Scheduling.Contracts.Abstractions;

    /// <summary>
    /// Interface for all notifications.
    /// </summary>
    public interface INotification : IEvent
    {
        /// <summary>
        /// Gets the packets that must be send as part of this notification.
        /// </summary>
        IList<IOutgoingPacket> Packets { get; }
    }
}
