// -----------------------------------------------------------------
// <copyright file="WorldLightChangedNotification.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Notifications
{
    using System;
    using System.Collections.Generic;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Packets.Outgoing;
    using Serilog;

    /// <summary>
    /// Class that represents a notification for a world light change.
    /// </summary>
    internal class WorldLightChangedNotification : Notification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorldLightChangedNotification"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="determineTargetConnectionsFunction">A function to determine the target connections of this notification.</param>
        /// <param name="arguments">The arguments for this notification.</param>
        public WorldLightChangedNotification(ILogger logger, Func<IEnumerable<IConnection>> determineTargetConnectionsFunction, WorldLightChangedNotificationArguments arguments)
            : base(logger)
        {
            determineTargetConnectionsFunction.ThrowIfNull(nameof(determineTargetConnectionsFunction));
            arguments.ThrowIfNull(nameof(arguments));

            this.TargetConnectionsFunction = determineTargetConnectionsFunction;
            this.Arguments = arguments;
        }

        /// <summary>
        /// Gets this notification's arguments.
        /// </summary>
        public WorldLightChangedNotificationArguments Arguments { get; }

        /// <summary>
        /// Gets the function for determining target connections for this notification.
        /// </summary>
        protected override Func<IEnumerable<IConnection>> TargetConnectionsFunction { get; }

        /// <summary>
        /// Finalizes the notification in preparation to it being sent.
        /// </summary>
        /// <param name="playerId">The id of the player which this notification is being prepared for.</param>
        /// <returns>A collection of <see cref="IOutgoingPacket"/>s, the ones to be sent.</returns>
        protected override IEnumerable<IOutgoingPacket> Prepare(uint playerId)
        {
            return new WorldLightPacket(this.Arguments.LightLevel, this.Arguments.LightColor).YieldSingleItem();
        }
    }
}