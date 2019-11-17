// -----------------------------------------------------------------
// <copyright file="AuctionsResultPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Packets.Outgoing
{
    using System.Collections.Generic;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Communications.Packets.Contracts.Abstractions;

    public class AuctionsResultPacket : IOutgoingPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuctionsResultPacket"/> class.
        /// </summary>
        /// <param name="results">The results of the auctions.</param>
        public AuctionsResultPacket(IList<IAuctionResult> results)
        {
            this.AuctionResults = results;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public byte PacketType => (byte)OutgoingManagementPacketType.NoError;

        /// <summary>
        /// Gets the auction results.
        /// </summary>
        public IList<IAuctionResult> AuctionResults { get; }

        /// <summary>
        /// Writes the packet to the message provided.
        /// </summary>
        /// <param name="message">The message to write this packet to.</param>
        public void WriteToMessage(INetworkMessage message)
        {
            message.WriteAuctionsResultPacket(this);
        }
    }
}
