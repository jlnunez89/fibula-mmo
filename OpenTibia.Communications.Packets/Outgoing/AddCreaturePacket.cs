// <copyright file="AddCreaturePacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Common.Utilities;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a packet with information about a creatue that was added to the game.
    /// </summary>
    public class AddCreaturePacket : IOutgoingPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddCreaturePacket"/> class.
        /// </summary>
        /// <param name="creature">The creature that was added.</param>
        /// <param name="asKnown">A value indicating whether the creature was added as a known creature or not.</param>
        /// <param name="removeThisCreatureId">An id of another creature to remove from the known list, and replace with this new creature.</param>
        public AddCreaturePacket(ICreature creature, bool asKnown, uint removeThisCreatureId)
        {
            creature.ThrowIfNull(nameof(creature));

            this.Creature = creature;
            this.AsKnown = asKnown;
            this.RemoveThisCreatureId = removeThisCreatureId;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public byte PacketType => (byte)OutgoingGamePacketType.AddAtStackpos;

        /// <summary>
        /// Gets a reference to the creature added.
        /// </summary>
        public ICreature Creature { get; }

        /// <summary>
        /// Gets a value indicating whether the creature was added as a known creature or not.
        /// </summary>
        public bool AsKnown { get; }

        /// <summary>
        /// Gets an id of another creature to remove from the known list, and replace with this new creature.
        /// </summary>
        public uint RemoveThisCreatureId { get; }

        /// <summary>
        /// Writes the packet to the message provided.
        /// </summary>
        /// <param name="message">The message to write this packet to.</param>
        public void WriteToMessage(INetworkMessage message)
        {
            message.WriteAddCreaturePacket(this);
        }
    }
}
