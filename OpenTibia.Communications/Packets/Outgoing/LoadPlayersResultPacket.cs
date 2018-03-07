using System.Collections.Generic;
using OpenTibia.Data.Models;
using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Communications.Packets.Outgoing
{
    public class LoadPlayersResultPacket : PacketOutgoing
    {
        public IList<PlayerModel> LoadedPlayers { get; set; }

        public override byte PacketType => (byte)ManagementOutgoingPacketType.NoType;

        public override void Add(NetworkMessage message)
        {
            message.AddByte(0x00);
            message.AddUInt32((uint)LoadedPlayers.Count);
           
            foreach(var player in LoadedPlayers)
            {
                message.AddString(player.Charname);
                message.AddUInt32((uint)player.Account_Id);
            }
        }

        public override void CleanUp()
        {
            LoadedPlayers = null;
        }
    }
}