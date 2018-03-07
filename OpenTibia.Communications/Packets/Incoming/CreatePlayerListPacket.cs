using System.Collections.Generic;
using OpenTibia.Data.Contracts;
using OpenTibia.Data.Models;
using OpenTibia.Server.Data;

namespace OpenTibia.Communications.Packets.Incoming
{
    public class CreatePlayerListPacket : PacketIncoming, IPlayerListInfo
    {
        public IList<IOnlinePlayer> PlayerList { get; set; }

        public CreatePlayerListPacket(NetworkMessage message) : base(message)
        {
        }

        public override void Parse(NetworkMessage message)
        {
            var count = message.GetUInt16();

            PlayerList = new List<IOnlinePlayer>();

            for (int i = 0; i < count; i++)
            {
                PlayerList.Add(new OnlinePlayer
                {
                    Name = message.GetString(),
                    Level = message.GetUInt16(),
                    Vocation = message.GetString()
                });
            }
        }
    }
}
