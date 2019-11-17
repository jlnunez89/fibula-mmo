// <copyright file="UpdateTilePacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;

    public class UpdateTilePacket : IOutgoingPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateTilePacket"/> class.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="descriptionBytes"></param>
        public UpdateTilePacket(Location location, byte[] descriptionBytes)
        {
            this.Location = location;
            this.DescriptionBytes = descriptionBytes;
        }

        public byte PacketType => (byte)OutgoingGamePacketType.TileUpdate;

        public Location Location { get; }

        public byte[] DescriptionBytes { get; }

        /// <summary>
        /// Writes the packet to the message provided.
        /// </summary>
        /// <param name="message">The message to write this packet to.</param>
        public void WriteToMessage(INetworkMessage message)
        {
            message.WriteUpdateTilePacket(this);
        }
    }
}
