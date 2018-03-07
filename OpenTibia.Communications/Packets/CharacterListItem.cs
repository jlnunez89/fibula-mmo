using System.Net;

namespace OpenTibia.Communications.Packets
{
    public class CharacterListItem : ICharacterListItem
    {
        public string Name { get; set; }
        public string World { get; set; }
        public byte[] Ip { get; set; }
        public ushort Port { get; set; }

        public CharacterListItem()
        {
            Name = string.Empty;
            World = string.Empty;
            Ip = null;
            Port = 0;
        }

        public CharacterListItem(string name, IPAddress ip, ushort port, string world = "RandomOT")
        {
            Name = name;
            World = world;
            Ip = ip.GetAddressBytes();
            Port = port;
        }
    }
}