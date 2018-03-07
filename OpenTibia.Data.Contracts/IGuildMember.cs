namespace OpenTibia.Data.Contracts
{
    public interface IGuildMember
    {
        int EntryId { get; set; }
        int AccountId { get; set; }
        short GuildId { get; set; }
        string GuildTitle { get; set; }
        string PlayerTitle { get; set; }
        byte Invitation { get; set; }
        int Timestamp { get; set; }
        byte Rank { get; set; }
    }
}
