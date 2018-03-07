namespace OpenTibia.Data.Contracts
{
    public interface IGuild
    {
        short GuildId { get; set; }
        string GuildName { get; set; }
        int GuildOwner { get; set; }
        string Description { get; set; }
        int Ts { get; set; }
        byte Ranks { get; set; }
        string Rank1 { get; set; }
        string Rank2 { get; set; }
        string Rank3 { get; set; }
        string Rank4 { get; set; }
        string Rank5 { get; set; }
        string Rank6 { get; set; }
        string Rank7 { get; set; }
        string Rank8 { get; set; }
        string Rank9 { get; set; }
        string Rank10 { get; set; }
        string Logo { get; set; }
    }
}
