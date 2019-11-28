// -----------------------------------------------------------------
// <copyright file="CreatureLightPacket.cs" company="2Dudes">
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
    using OpenTibia.Server.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a creature light packet.
    /// </summary>
    public class CreatureLightPacket : IOutgoingPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureLightPacket"/> class.
        /// </summary>
        /// <param name="creature">The creature reference.</param>
        public CreatureLightPacket(ICreature creature)
        {
            this.Creature = creature;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public byte PacketType => (byte)OutgoingGamePacketType.CreatureLight;

        /// <summary>
        /// Gets the creature reference.
        /// </summary>
        public ICreature Creature { get; }

        /// <summary>
        /// Writes the packet to the message provided.
        /// </summary>
        /// <param name="message">The message to write this packet to.</param>
        public void WriteToMessage(INetworkMessage message)
        {
            message.WriteCreatureLightPacket(this);
        }
    }
}
