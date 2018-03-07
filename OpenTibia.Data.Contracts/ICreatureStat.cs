namespace OpenTibia.Data.Contracts
{
    public interface ICreatureStat
    {
        string Name { get; set; }
        int KilledBy { get; set; }
        int Killed { get; set; }
        long Time { get; set; }
    }
}
