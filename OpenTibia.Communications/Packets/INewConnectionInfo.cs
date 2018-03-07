
namespace OpenTibia.Communications.Packets
{
    public interface INewConnectionInfo
    {
        ushort Os { get; set; }
        ushort Version { get; set; }
    }
}
