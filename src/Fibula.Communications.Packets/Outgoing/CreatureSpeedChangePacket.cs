// -----------------------------------------------------------------
// <copyright file="CreatureSpeedChangePacket.cs" company="2Dudes">
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
    /// Class that represents a creature speed change packet.
    /// </summary>
    public class CreatureSpeedChangePacket : IOutboundPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureSpeedChangePacket"/> class.
        /// </summary>
        /// <param name="creature">The creature reference.</param>
        public CreatureSpeedChangePacket(ICreature creature)
        {
            this.Creature = creature;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public OutgoingPacketType PacketType => OutgoingPacketType.CreatureSpeedChange;

        /// <summary>
        /// Gets the creature reference.
        /// </summary>
        public ICreature Creature { get; }
    }
}
