using OpenTibia.Data.Contracts;
using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;
using OpenTibia.Server.Data.Models.Structs;

namespace OpenTibia.Communications.Packets.Outgoing
{
    public class CreatureSpeechPacket : PacketOutgoing
    {
        public override byte PacketType => (byte)GameOutgoingPacketType.CreatureSpeech;

        public string SenderName { get; set; }
        public SpeechType SpeechType { get; set; }
        public string Text { get; set;}
        public Location Location { get; set; }
        public ChatChannel ChannelId { get; set; }
        public uint Time { get; set; }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);
            message.AddUInt32(0);
            message.AddString(SenderName);
            message.AddByte((byte)SpeechType);

            switch (SpeechType)
            {
                case SpeechType.Say:
                case SpeechType.Whisper:
                case SpeechType.Yell:
                case SpeechType.MonsterSay:
                //case SpeechType.MonsterYell:
                    message.AddLocation(Location);
                    break;
                //case SpeechType.ChannelRed:
                //case SpeechType.ChannelRedAnonymous:
                //case SpeechType.ChannelOrange:
                case SpeechType.ChannelYellow:
                //case SpeechType.ChannelWhite:
                    message.AddUInt16((ushort)ChannelId);
                    break;
                case SpeechType.RuleViolationReport:
                    message.AddUInt32(Time);
                    break;
                default:
                    break;

            }

            message.AddString(Text);
        }

        public override void CleanUp()
        {
            // No references to clear.
        }
    }
}
