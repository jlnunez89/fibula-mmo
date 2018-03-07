namespace OpenTibia.Data.Contracts
{
    public interface IStat
    {
        int PlayersOnline { get; set; }

        int RecordOnline { get; set; }
    }
}
