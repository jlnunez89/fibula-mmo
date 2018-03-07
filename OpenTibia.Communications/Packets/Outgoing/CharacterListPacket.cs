using System.Collections.Generic;
using System.Linq;
using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Communications.Packets.Outgoing
{
    public class CharacterListPacket : PacketOutgoing
    {
        public IEnumerable<ICharacterListItem> Characters { get; set; }
        public ushort PremiumDaysLeft { get; set; }

        public override byte PacketType => (byte)ManagementOutgoingPacketType.CharacterList;

        public CharacterListPacket()
        {
            Characters = Enumerable.Empty<ICharacterListItem>();
            PremiumDaysLeft = 0;
        }

        public CharacterListPacket(IEnumerable<ICharacterListItem> characters, ushort premDays)
        {
            Characters = characters;
            PremiumDaysLeft = premDays;
        }

        public override void Add(NetworkMessage message)
        {
            message.AddByte(PacketType);
            message.AddByte((byte)Characters.Count());

            foreach (ICharacterListItem character in Characters)
            {
                message.AddString(character.Name);
                message.AddString(character.World);
                message.AddBytes(character.Ip);
                message.AddUInt16(character.Port);
            }

            message.AddUInt16(PremiumDaysLeft);
        }

        public override void CleanUp()
        {
            Characters = null;
        }
    }
}
