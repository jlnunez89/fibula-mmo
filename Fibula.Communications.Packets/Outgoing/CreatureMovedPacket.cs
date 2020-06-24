// -----------------------------------------------------------------
// <copyright file="CreatureMovedPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
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
        public byte PacketType => (byte)OutgoingGamePacketType.CreatureMoved;

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
