using System;
using OpenTibia.Data.Contracts;
using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Communications.Packets.Outgoing
{
    public class PlayerInventoryPacket : PacketOutgoing
    {
        public override byte PacketType => (byte)GameOutgoingPacketType.InventoryItem;

        public IPlayer Player { get; set; }

        public override void Add(NetworkMessage message)
        {            
            var addInventoryItem = new Action<Slot>(slot =>
            {
                if (Player.Inventory[(byte)slot] == null)
                {
                    message.AddByte((byte)GameOutgoingPacketType.InventoryEmpty);
                    message.AddByte((byte)slot);
                }
                else
                {
                    message.AddByte((byte)GameOutgoingPacketType.InventoryItem);
                    message.AddByte((byte)slot);
                    message.AddItem(Player.Inventory[(byte)slot]);
                }
            });

            addInventoryItem(Slot.Head);
            addInventoryItem(Slot.Necklace);
            addInventoryItem(Slot.Backpack);
            addInventoryItem(Slot.Body);
            addInventoryItem(Slot.Right);
            addInventoryItem(Slot.Left);
            addInventoryItem(Slot.Legs);
            addInventoryItem(Slot.Feet);
            addInventoryItem(Slot.Ring);
            addInventoryItem(Slot.Ammo);
        }

        public override void CleanUp()
        {
            this.Player = null;
        }
    }
}
