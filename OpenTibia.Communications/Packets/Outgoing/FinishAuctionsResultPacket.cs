using System.Collections.Generic;
using OpenTibia.Data.Models;
using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Communications.Packets.Outgoing
{
    public class FinishAuctionsResultPacket : PacketOutgoing
    {
        public override byte PacketType => (byte)ManagementOutgoingPacketType.NoType;

        public IList<AssignedHouse> Houses { get; set; }
        
        public override void Add(NetworkMessage message)
        {
            message.AddByte(0x00); // Should always be 0 for this packet; means there was no error.
            message.AddUInt16((ushort)Houses.Count);
            
            foreach(var house in Houses)
            {
                message.AddUInt16((ushort)house.HouseId);
                message.AddUInt32((uint)house.PlayerId);
                message.AddString(house.OwnerString);
                message.AddUInt32((uint)house.Gold);
            }
        }

        public override void CleanUp()
        {
            Houses = null;
        }
    }
}
