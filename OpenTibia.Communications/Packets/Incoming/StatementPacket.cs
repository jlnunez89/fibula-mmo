using System;
using System.Collections.Generic;
using OpenTibia.Server.Data;

namespace OpenTibia.Communications.Packets.Incoming
{
    public class StatementPacket : PacketIncoming, IStatementInfo
    {
        public uint Unknown { get; set; }
        public uint StatementId { get; set; }
        public ushort Count { get; set; }
        public IList<Tuple<uint, uint, string, string>> Data { get; set; }

        public StatementPacket(NetworkMessage message)
            : base(message)
        {
        }

        public override void Parse(NetworkMessage message)
        {
            Unknown = message.GetUInt32();
            StatementId = message.GetUInt32();
            Count = message.GetUInt16();

            Data = new List<Tuple<uint, uint, string, string>>();

            for(int i = 0; i < Count; i++)
            {
                message.GetUInt32(); // ignore, this is the same statementId

                // timestamp, playerId, channel, message
                Data.Add(new Tuple<uint, uint, string, string>(message.GetUInt32(), message.GetUInt32(), message.GetString(), message.GetString()));
            }
        }
    }
}
