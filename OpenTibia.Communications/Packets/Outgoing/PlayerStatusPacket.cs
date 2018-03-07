using System;
using OpenTibia.Data.Contracts;
using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Communications.Packets.Outgoing
{
    public class PlayerStatusPacket : PacketOutgoing
    {
        public override byte PacketType => (byte)GameOutgoingPacketType.PlayerStatus;

        public IPlayer Player { get; set; }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);

            message.AddUInt16((ushort)Math.Min(ushort.MaxValue, Player.Hitpoints));
            message.AddUInt16((ushort)Math.Min(ushort.MaxValue, Player.MaxHitpoints));
            message.AddUInt16(Convert.ToUInt16(Player.CarryStrength));
            
            message.AddUInt32(Math.Min(0x7FFFFFFF, Player.Experience)); //Experience: Client debugs after 2,147,483,647 exp

            message.AddUInt16(Player.Level);
            message.AddByte(Player.LevelPercent);
            message.AddUInt16((ushort)Math.Min(ushort.MaxValue, Player.Manapoints));
            message.AddUInt16((ushort)Math.Min(ushort.MaxValue, Player.MaxManapoints));
            message.AddByte(Player.GetSkillInfo(SkillType.Magic));
            message.AddByte(Player.GetSkillPercent(SkillType.Magic));

            message.AddByte(Player.SoulPoints);
        }

        public override void CleanUp()
        {
            Player = null;
        }
    }
}
