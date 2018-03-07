namespace OpenTibia.Data.Contracts
{
    public interface ICipCreature
    {
        byte Id { get; set; }
        string Race { get; set; }
        string Plural { get; set; }
        string Description { get; set; }
    }
}
