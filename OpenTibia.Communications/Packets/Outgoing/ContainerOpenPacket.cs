using System;
using System.Collections.Generic;
using System.Linq;
using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Communications.Packets.Outgoing
{
    public class ContainerOpenPacket : PacketOutgoing
    {
        public byte ContainerId { get; set; }
        public ushort ClientItemId { get; set; }
        public string Name { get; set; }
        public byte Volume { get; set; }
        public bool HasParent { get; set; }
        public IList<IItem> Contents { get; set; }

        public override byte PacketType => (byte)GameOutgoingPacketType.ContainerOpen;

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);
            
            message.AddByte(ContainerId);
            message.AddUInt16(ClientItemId);
            message.AddString(Name);
            message.AddByte(Volume);
            message.AddByte(Convert.ToByte(HasParent ? 0x01 : 0x00));
            message.AddByte(Convert.ToByte(Contents.Count));

            foreach (var item in Contents.Reverse())
            {
                message.AddItem(item);
            }
        }

        public override void CleanUp()
        {
            Contents = null;
        }
    }
}