// -----------------------------------------------------------------
// <copyright file="CreatureMovedPacket.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Packets.Outgoing
{
    using Fibula.Common.Contracts.Structs;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Contracts.Enumerations;

    /// <summary>
    /// Class that represents a packet for when a creature has moved.
    /// </summary>
    public class CreatureMovedPacket : IOutboundPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureMovedPacket"/> class.
        /// </summary>
        /// <param name="fromLocation">The location from which the move happened.</param>
        /// <param name="fromStackpos">The position in the stack within the location from which the move happened.</param>
        /// <param name="toLocation">The location to which the move happened.</param>
        public CreatureMovedPacket(Location fromLocation, byte fromStackpos, Location toLocation)
        {
            this.FromLocation = fromLocation;
            this.FromStackpos = fromStackpos;
            this.ToLocation = toLocation;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public OutgoingPacketType PacketType => OutgoingPacketType.CreatureMoved;

        /// <summary>
        /// Gets the location from which the creature moved from.
        /// </summary>
        public Location FromLocation { get; }

        /// <summary>
        /// Gets the position in the stack of the creatue in the source location.
        /// </summary>
        public byte FromStackpos { get; }

        /// <summary>
        /// Gets the location to which the creature moved.
        /// </summary>
        public Location ToLocation { get; }
    }
}
