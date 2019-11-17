// <copyright file="LoadPlayersResultPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using System.Collections.Generic;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Communications.Packets.Contracts.Abstractions;

    public class LoadPlayersResultPacket : IOutgoingPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoadPlayersResultPacket"/> class.
        /// </summary>
        /// <param name="loadedPlayers"></param>
        public LoadPlayersResultPacket(IList<IPlayerLoaded> loadedPlayers)
        {
            this.LoadedPlayers = loadedPlayers;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public byte PacketType => (byte)OutgoingManagementPacketType.NoError;

        public IList<IPlayerLoaded> LoadedPlayers { get; }

        /// <summary>
        /// Writes the packet to the message provided.
        /// </summary>
        /// <param name="message">The message to write this packet to.</param>
        public void WriteToMessage(INetworkMessage message)
        {
            message.WriteLoadPlayersResultPacket(this);
        }
    }
}