// -----------------------------------------------------------------
// <copyright file="CreatureMovedPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;

    public class CreatureMovedPacket : IOutgoingPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureMovedPacket"/> class.
        /// </summary>
        /// <param name="fromLocation"></param>
        /// <param name="fromStackpos"></param>
        /// <param name="toLocation"></param>
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
        /// Gets the tile source location.
        /// </summary>
        public Location FromLocation { get; }

        /// <summary>
        /// Gets the position in the stack of the creatue in the source location.
        /// </summary>
        public byte FromStackpos { get; }

        /// <summary>
        /// Gets the tile target location.
        /// </summary>
        public Location ToLocation { get; }

        /// <summary>
        /// Writes the packet to the message provided.
        /// </summary>
        /// <param name="message">The message to write this packet to.</param>
        public void WriteToMessage(INetworkMessage message)
        {
            message.WriteCreatureMovedPacket(this);
        }
    }
}
