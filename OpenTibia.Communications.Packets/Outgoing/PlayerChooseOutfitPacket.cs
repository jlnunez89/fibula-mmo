// <copyright file="PlayerChooseOutfitPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;

    public class PlayerChooseOutfitPacket : IOutgoingPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerChooseOutfitPacket"/> class.
        /// </summary>
        /// <param name="currentOutfit"></param>
        /// <param name="chooseFromId"></param>
        /// <param name="chooseToId"></param>
        public PlayerChooseOutfitPacket(Outfit currentOutfit, ushort chooseFromId, ushort chooseToId)
        {
            this.CurrentOutfit = currentOutfit;
            this.ChooseFromId = chooseFromId;
            this.ChooseToId = chooseToId;
        }

        public byte PacketType => (byte)OutgoingGamePacketType.OutfitWindow;

        public Outfit CurrentOutfit { get; }

        public ushort ChooseFromId { get; }

        public ushort ChooseToId { get; }

        /// <summary>
        /// Writes the packet to the message provided.
        /// </summary>
        /// <param name="message">The message to write this packet to.</param>
        public void WriteToMessage(INetworkMessage message)
        {
            message.WritePlayerChooseOutfitPacket(this);
        }
    }
}
