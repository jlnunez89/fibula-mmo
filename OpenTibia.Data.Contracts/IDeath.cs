namespace OpenTibia.Data.Contracts
{
    public interface IDeath
    {
        int RecordId { get; set; }
        int PlayerId { get; set; }
        int Level { get; set; }
        byte ByPeekay { get; set; }
        int PeekayId { get; set; }
        string CreatureString { get; set; }
        byte Unjust { get; set; }
        long Timestamp { get; set; }
    }
}
