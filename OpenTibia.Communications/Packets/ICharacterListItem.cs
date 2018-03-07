namespace OpenTibia.Communications.Packets
{
    public interface ICharacterListItem
    {
        string Name { get; set; }
        string World { get; set; }
        byte[] Ip { get; set; }
        ushort Port { get; set; }
    }
}