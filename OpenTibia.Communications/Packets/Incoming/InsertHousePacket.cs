using OpenTibia.Server.Data;

namespace OpenTibia.Communications.Packets.Incoming
{
    public class InsertHousePacket : PacketIncoming, IInsertHouseInfo
    {
        public ushort Count { get; set; }
        public ushort HouseId { get; set; }
        public string Name { get; set; }
        public uint Rent { get; set; }
        public string Description { get; set; }
        public ushort SquareMeters { get; set; }
        public ushort EntranceX { get; set; }
        public ushort EntranceY { get; set; }
        public byte EntranceZ { get; set; }
        public string Town { get; set; }
        public byte IsGuildHouse { get; set; }

        public InsertHousePacket(NetworkMessage message)
            : base(message)
        {
        }

        public override void Parse(NetworkMessage message)
        {
            Count = message.GetUInt16();
            HouseId = message.GetUInt16();
            Name = message.GetString();
            Rent = message.GetUInt32();
            Description = message.GetString();
            SquareMeters = message.GetUInt16();
            EntranceX = message.GetUInt16();
            EntranceY = message.GetUInt16();
            EntranceZ = message.GetByte();
            Town = message.GetString();
            IsGuildHouse = message.GetByte();
        }
    }
}