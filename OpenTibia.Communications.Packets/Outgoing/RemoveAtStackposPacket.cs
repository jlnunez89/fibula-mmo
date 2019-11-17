// <copyright file="RemoveAtStackposPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;

    public class RemoveAtStackposPacket : IOutgoingPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveAtStackposPacket"/> class.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="stackpos"></param>
        public RemoveAtStackposPacket(Location location, byte stackpos)
        {
            this.Location = location;
            this.Stackpos = stackpos;
        }

        public byte PacketType => (byte)OutgoingGamePacketType.RemoveAtStackpos;

        public Location Location { get; }

        public byte Stackpos { get; }

        /// <summary>
        /// Writes the packet to the message provided.
        /// </summary>
        /// <param name="message">The message to write this packet to.</param>
        public void WriteToMessage(INetworkMessage message)
        {
            message.WriteRemoveAtStackposPacket(this);
        }
    }
}
