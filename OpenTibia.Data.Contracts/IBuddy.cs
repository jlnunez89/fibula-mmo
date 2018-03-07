namespace OpenTibia.Data.Contracts
{
    public interface IBuddy
    {
        int EntryId { get; set; }
        int AccountNr { get; set; }
        int BuddyId { get; set; }
        string BuddyString { get; set; }
        long Timestamp { get; set; }
        int InitiatingId { get; set; }
    }
}
