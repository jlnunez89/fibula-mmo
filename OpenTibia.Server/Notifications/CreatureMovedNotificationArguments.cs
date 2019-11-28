// -----------------------------------------------------------------
// <copyright file="CreatureMovedNotificationArguments.cs" company="2Dudes">
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
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Structs;

    /// <summary>
    /// Class that represents arguments for the creature moving notification.
    /// </summary>
    internal class CreatureMovedNotificationArguments : INotificationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureMovedNotificationArguments"/> class.
        /// </summary>
        /// <param name="creatureId">The id of the creature moving.</param>
        /// <param name="fromLocation">The location from which the creature is moving.</param>
        /// <param name="fromStackPos">The stack position from where the creature moving.</param>
        /// <param name="toLocation">The location to which the creature is moving.</param>
        /// <param name="toStackPos">The stack position to which the creature is moving.</param>
        /// <param name="wasTeleport">A value indicating whether this movement was a teleportation.</param>
        public CreatureMovedNotificationArguments(uint creatureId, Location fromLocation, byte fromStackPos, Location toLocation, byte toStackPos, bool wasTeleport)
        {
            var locationDiff = fromLocation - toLocation;

            this.CreatureId = creatureId;
            this.OldLocation = fromLocation;
            this.OldStackPosition = fromStackPos;
            this.NewLocation = toLocation;
            this.NewStackPosition = toStackPos;
            this.WasTeleport = wasTeleport || locationDiff.MaxValueIn3D > 1;
        }

        /// <summary>
        /// Gets a value indicating whether this movement was a teleportation.
        /// </summary>
        public bool WasTeleport { get; }

        /// <summary>
        /// Gets the location from which the creature is moving.
        /// </summary>
        public Location OldLocation { get; }

        /// <summary>
        /// Gets the stack position from where the creature moving.
        /// </summary>
        public byte OldStackPosition { get; }

        /// <summary>
        /// Gets the stack position to which the creature is moving.
        /// </summary>
        public byte NewStackPosition { get; }

        /// <summary>
        /// Gets the location to which the creature is moving.
        /// </summary>
        public Location NewLocation { get; }

        /// <summary>
        /// Gets the id of the creature moving.
        /// </summary>
        public uint CreatureId { get; }
    }
}