using OpenTibia.Data.Contracts;
using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Communications.Packets.Outgoing
{
    public class PlayerSkillsPacket : PacketOutgoing
    {
        public override byte PacketType => (byte)GameOutgoingPacketType.PlayerSkills;

        public IPlayer Player { get; set; }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);

            message.AddByte(Player.GetSkillInfo(SkillType.Fist));
            message.AddByte(Player.GetSkillPercent(SkillType.Fist));

            message.AddByte(Player.GetSkillInfo(SkillType.Club));
            message.AddByte(Player.GetSkillPercent(SkillType.Club));

            message.AddByte(Player.GetSkillInfo(SkillType.Sword));
            message.AddByte(Player.GetSkillPercent(SkillType.Sword));

            message.AddByte(Player.GetSkillInfo(SkillType.Axe));
            message.AddByte(Player.GetSkillPercent(SkillType.Axe));

            message.AddByte(Player.GetSkillInfo(SkillType.Ranged));
            message.AddByte(Player.GetSkillPercent(SkillType.Ranged));

            message.AddByte(Player.GetSkillInfo(SkillType.Shield));
            message.AddByte(Player.GetSkillPercent(SkillType.Shield));

            message.AddByte(Player.GetSkillInfo(SkillType.Fishing));
            message.AddByte(Player.GetSkillPercent(SkillType.Fishing));
        }

        public override void CleanUp()
        {
            Player = null;
        }
    }
}
