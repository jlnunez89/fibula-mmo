// -----------------------------------------------------------------
// <copyright file="AddCreaturePacket.cs" company="2Dudes">
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
    using Fibula.Common.Utilities;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Contracts.Enumerations;
    using Fibula.Creatures.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a packet with information about a creatue that was added to the game.
    /// </summary>
    public class AddCreaturePacket : IOutboundPacket
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
    }
}
