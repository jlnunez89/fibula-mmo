
namespace OpenTibia.Communications.Packets
{
    public interface IFinishAuctionsInfo
    {
        uint VictimId { get; set; }
        ushort VictimLevel { get; set; }
        uint KillerId { get; set; }
        string KillerName { get; set; }
        byte Unjustified { get; set; }
        uint Timestamp { get; set; }
    }
}
