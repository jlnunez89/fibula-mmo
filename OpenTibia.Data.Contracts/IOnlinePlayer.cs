namespace OpenTibia.Data.Contracts
{
    public interface IOnlinePlayer
    {
        string Name { get; set; }
        int Level { get; set; }
        string Vocation { get; set; }
    }
}
