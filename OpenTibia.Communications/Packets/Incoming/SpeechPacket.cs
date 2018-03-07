using System;
using OpenTibia.Data.Contracts;
using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Models.Structs;

namespace OpenTibia.Communications.Packets.Incoming
{
    public class SpeechPacket : PacketIncoming
    {
        public Speech Speech { get; private set; }

        public SpeechPacket(NetworkMessage message)
            : base(message)
        {
        }

        public override void Parse(NetworkMessage message)
        {
            var type = message.GetByte();

            try
            {
                switch (Speech.Type)
                {
                    case SpeechType.Private:
                    //case SpeechType.PrivateRed:
                    case SpeechType.RuleViolationAnswer:
                        Speech = new Speech
                        {
                            Type = (SpeechType)type,
                            Receiver = message.GetString(),
                            Message = message.GetString()
                        };
                        break;
                    case SpeechType.ChannelYellow:
                        //case SpeechType.ChannelRed:
                        //case SpeechType.ChannelRedAnonymous:
                        //case SpeechType.ChannelWhite:
                        Speech = new Speech
                        {
                            Type = (SpeechType)type,
                            ChannelId = (ChatChannel)message.GetUInt16(),
                            Message = message.GetString()
                        };
                        break;
                    default:
                        Speech = new Speech
                        {
                            Type = (SpeechType)type,
                            Message = message.GetString()
                        };
                        break;
                }
            }
            catch
            {
                Console.WriteLine($"Unknown speech type {type}.");
            }
        }
    }
}
