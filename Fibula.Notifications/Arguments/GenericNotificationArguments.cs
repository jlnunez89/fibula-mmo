// -----------------------------------------------------------------
// <copyright file="GenericNotificationArguments.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Notifications.Arguments
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Notifications.Contracts.Abstractions;

    /// <summary>
    /// Class that represents generic notification arguments.
    /// </summary>
    public class GenericNotificationArguments : INotificationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericNotificationArguments"/> class.
        /// </summary>
        /// <param name="outgoingPackets">The packets to send as part of this notification.</param>
        public GenericNotificationArguments(params IOutboundPacket[] outgoingPackets)
        {
            if (outgoingPackets == null || !outgoingPackets.Any())
            {
                throw new ArgumentNullException(nameof(outgoingPackets));
            }

            this.OutgoingPackets = outgoingPackets;
        }

        /// <summary>
        /// Gets the packets to be included in this notification.
        /// </summary>
        public IEnumerable<IOutboundPacket> OutgoingPackets { get; }
    }
}
