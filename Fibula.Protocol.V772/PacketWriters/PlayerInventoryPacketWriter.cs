// -----------------------------------------------------------------
// <copyright file="PlayerInventoryPacketWriter.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Protocol772.PacketWriters
{
    using System.Linq;
    using Fibula.Communications;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Contracts.Enumerations;
    using Fibula.Communications.Packets.Outgoing;
    using Fibula.Items.Contracts.Abstractions;
    using Fibula.Server.Contracts.Enumerations;
    using Fibula.Server.Protocol772;
    using Serilog;

    /// <summary>
    /// Class that represents a player's inventory packet writer for the game server.
    /// </summary>
    public class PlayerInventoryPacketWriter : BasePacketWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerInventoryPacketWriter"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        public PlayerInventoryPacketWriter(ILogger logger)
            : base(logger)
        {
        }

        /// <summary>
        /// Writes a packet to the given <see cref="INetworkMessage"/>.
        /// </summary>
        /// <param name="packet">The packet to write.</param>
        /// <param name="message">The message to write into.</param>
        public override void WriteToMessage(IOutboundPacket packet, ref INetworkMessage message)
        {
            if (!(packet is PlayerInventoryPacket playerInventoryPacket))
            {
                this.Logger.Warning($"Invalid packet {packet.GetType().Name} routed to {this.GetType().Name}");

                return;
            }

            this.AddInventoryItem(ref message, Slot.Head, playerInventoryPacket.Player.Inventory[(byte)Slot.Head] as IContainerItem);
            this.AddInventoryItem(ref message, Slot.Neck, playerInventoryPacket.Player.Inventory[(byte)Slot.Neck] as IContainerItem);
            this.AddInventoryItem(ref message, Slot.Back, playerInventoryPacket.Player.Inventory[(byte)Slot.Back] as IContainerItem);
            this.AddInventoryItem(ref message, Slot.Body, playerInventoryPacket.Player.Inventory[(byte)Slot.Body] as IContainerItem);
            this.AddInventoryItem(ref message, Slot.RightHand, playerInventoryPacket.Player.Inventory[(byte)Slot.RightHand] as IContainerItem);
            this.AddInventoryItem(ref message, Slot.LeftHand, playerInventoryPacket.Player.Inventory[(byte)Slot.LeftHand] as IContainerItem);
            this.AddInventoryItem(ref message, Slot.Legs, playerInventoryPacket.Player.Inventory[(byte)Slot.Legs] as IContainerItem);
            this.AddInventoryItem(ref message, Slot.Feet, playerInventoryPacket.Player.Inventory[(byte)Slot.Feet] as IContainerItem);
            this.AddInventoryItem(ref message, Slot.Ring, playerInventoryPacket.Player.Inventory[(byte)Slot.Ring] as IContainerItem);
            this.AddInventoryItem(ref message, Slot.Ammo, playerInventoryPacket.Player.Inventory[(byte)Slot.Ammo] as IContainerItem);
        }

        private void AddInventoryItem(ref INetworkMessage message, Slot slot, IContainerItem slotContainer)
        {
            var itemInContainer = slotContainer?.Content.FirstOrDefault();

            if (itemInContainer == null)
            {
                message.AddByte((byte)OutgoingGamePacketType.InventoryEmpty);
                message.AddByte((byte)slot);
            }
            else
            {
                message.AddByte((byte)OutgoingGamePacketType.InventoryItem);
                message.AddByte((byte)slot);
                message.AddItem(itemInContainer);
            }
        }
    }
}
