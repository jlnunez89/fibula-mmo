// -----------------------------------------------------------------
// <copyright file="CreatureTurnedPacket.cs" company="2Dudes">
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
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Contracts.Enumerations;
    using Fibula.Creatures.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a packet for when a creature has turned.
    /// </summary>
    public class CreatureTurnedPacket : IOutboundPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureTurnedPacket"/> class.
        /// </summary>
        /// <param name="creature">The creature that turned.</param>
        /// <param name="creatureStackPosition">The position in the stack of the creature that turned.</param>
        public CreatureTurnedPacket(ICreature creature, byte creatureStackPosition)
        {
            this.Creature = creature;
            this.StackPosition = creatureStackPosition;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public OutgoingPacketType PacketType => OutgoingPacketType.UpdateThing;

        /// <summary>
        /// Gets the creature that turned.
        /// </summary>
        public ICreature Creature { get; }

        /// <summary>
        /// Gets the position in the stack of the creatue.
        /// </summary>
        public byte StackPosition { get; }
    }
}
