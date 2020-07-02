// -----------------------------------------------------------------
// <copyright file="Client.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fibula.Client.Contracts.Abstractions;
    using Fibula.Client.Contracts.Models;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Utilities;
    using Fibula.Communications.Contracts.Abstractions;

    /// <summary>
    /// Class that implements an <see cref="IClient"/> for any sort of connection.
    /// </summary>
    public class Client : IClient
    {
        /// <summary>
        /// Stores the set of creatures that are known to this player.
        /// </summary>
        private readonly IDictionary<uint, long> knownCreatures;

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class.
        /// </summary>
        /// <param name="connection">The connection that this client uses.</param>
        public Client(IConnection connection)
        {
            connection.ThrowIfNull(nameof(connection));

            this.knownCreatures = new Dictionary<uint, long>();

            this.Connection = connection;
            this.ClientInformation = new ClientInformation()
            {
                Type = AgentType.Undefined,
                Version = "Unknown",
            };
        }

        /// <summary>
        /// Gets a value indicating whether this client is idle.
        /// </summary>
        public bool IsIdle => this.Connection.IsOrphaned;

        /// <summary>
        /// Gets the connection enstablished by this client.
        /// </summary>
        public IConnection Connection { get; }

        /// <summary>
        /// Gets the information about the client on the other side of this connection.
        /// </summary>
        public ClientInformation ClientInformation { get; }

        /// <summary>
        /// Gets or sets the id of the player that this client is tied to.
        /// </summary>
        public uint PlayerId { get; set; }

        /// <summary>
        /// Sends the packets supplied over the <see cref="Connection"/>.
        /// </summary>
        /// <param name="packetsToSend">The packets to send.</param>
        public void Send(IEnumerable<IOutboundPacket> packetsToSend)
        {
            if (packetsToSend == null || !packetsToSend.Any())
            {
                return;
            }

            this.Connection.Send(packetsToSend);
        }

        /// <summary>
        /// Associates this connection with a player.
        /// </summary>
        /// <param name="toPlayerId">The Id of the player that the connection will be associated to.</param>
        public void AssociateToPlayer(uint toPlayerId)
        {
            this.PlayerId = toPlayerId;
        }

        /// <summary>
        /// Checks if this player knows the given creature.
        /// </summary>
        /// <param name="creatureId">The id of the creature to check.</param>
        /// <returns>True if the player knows the creature, false otherwise.</returns>
        public bool KnowsCreatureWithId(uint creatureId)
        {
            return this.knownCreatures.ContainsKey(creatureId);
        }

        /// <summary>
        /// Adds the given creature to this player's known collection.
        /// </summary>
        /// <param name="creatureId">The id of the creature to add to the known creatures collection.</param>
        public void AddKnownCreature(uint creatureId)
        {
            this.knownCreatures[creatureId] = DateTimeOffset.UtcNow.Ticks;
        }

        /// <summary>
        /// Chooses a creature to remove from this player's known creatures collection, if it has reached the collection size limit.
        /// </summary>
        /// <returns>The id of the chosen creature, if any, or <see cref="uint.MinValue"/> if no creature was chosen.</returns>
        public uint ChooseCreatureToRemoveFromKnownSet()
        {
            // If the buffer is full we need to choose a victim.
            while (this.knownCreatures.Count == IClient.KnownCreatureLimit)
            {
                // ToList() prevents modifiying an enumerating collection in the rare case we hit an exception down there.
                foreach (var candidate in this.knownCreatures.OrderBy(kvp => kvp.Value).ToList())
                {
                    if (this.knownCreatures.Remove(candidate.Key))
                    {
                        return candidate.Key;
                    }
                }
            }

            return uint.MinValue;
        }
    }
}
