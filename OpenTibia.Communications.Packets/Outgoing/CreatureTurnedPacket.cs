// <copyright file="CreatureTurnedPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Abstractions;

    public class CreatureTurnedPacket : IOutgoingPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureTurnedPacket"/> class.
        /// </summary>
        /// <param name="creature"></param>
        public CreatureTurnedPacket(ICreature creature)
        {
            this.Creature = creature;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public byte PacketType => (byte)OutgoingGamePacketType.TransformThing;

        public ICreature Creature { get; }

        /// <summary>
        /// Writes the packet to the message provided.
        /// </summary>
        /// <param name="message">The message to write this packet to.</param>
        public void WriteToMessage(INetworkMessage message)
        {
            message.WriteCreatureTurnedPacket(this);
        }
    }
}
