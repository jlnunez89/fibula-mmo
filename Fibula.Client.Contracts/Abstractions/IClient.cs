// -----------------------------------------------------------------
// <copyright file="IClient.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Client.Contracts.Abstractions
{
    using System.Collections.Generic;
    using Fibula.Client.Contracts.Models;
    using Fibula.Communications.Contracts.Abstractions;

    /// <summary>
    /// Interface for connections.
    /// </summary>
    public interface IClient
    {
        /// <summary>
        /// The limit of creatures that a client can keep track of.
        /// </summary>
        public const int KnownCreatureLimit = 150;

        /// <summary>
        /// Gets a value indicating whether this client is idle.
        /// </summary>
        bool IsIdle { get; }

        /// <summary>
        /// Gets the id of the player that this client is tied to.
        /// </summary>
        public uint PlayerId { get; }

        /// <summary>
        /// Gets the connection enstablished by this client.
        /// </summary>
        IConnection Connection { get; }

        /// <summary>
        /// Gets the information about the client on the other side of this connection.
        /// </summary>
        ClientInformation ClientInformation { get; }

        /// <summary>
        /// Sends the packets supplied over the <see cref="Connection"/>.
        /// </summary>
        /// <param name="packetsToSend">The packets to send.</param>
        void Send(IEnumerable<IOutboundPacket> packetsToSend);

        /// <summary>
        /// Associates this connection with a player.
        /// </summary>
        /// <param name="toPlayerId">The Id of the player that the connection will be associated to.</param>
        void AssociateToPlayer(uint toPlayerId);

        /// <summary>
        /// Checks if this client knows the given creature.
        /// </summary>
        /// <param name="creatureId">The id of the creature to check.</param>
        /// <returns>True if the client knows the creature, false otherwise.</returns>
        bool KnowsCreatureWithId(uint creatureId);

        /// <summary>
        /// Adds the given creature to this client's known creatures collection.
        /// </summary>
        /// <param name="creatureId">The id of the creature to add to the known creatures collection.</param>
        void AddKnownCreature(uint creatureId);

        /// <summary>
        /// Chooses a creature to remove from this client's known creatures collection, if it has reached the collection size limit.
        /// </summary>
        /// <returns>The id of the chosen creature, if any, or <see cref="uint.MinValue"/> if no creature was chosen.</returns>
        uint ChooseCreatureToRemoveFromKnownSet();
    }
}