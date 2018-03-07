
namespace OpenTibia.Communications.Packets
{
    public interface ICharacterDeathInfo
    {
        int VictimId { get; set; }
        short VictimLevel { get; set; }
        int KillerId { get; set; }
        string KillerName { get; set; }
        byte Unjustified { get; set; }
        long Timestamp { get; set; }
    }
}
