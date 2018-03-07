namespace OpenTibia.Data.Contracts
{
    public interface IRandPlayer
    {
        int RandId { get; set; }
        int AccountId { get; set; }
        int Order { get; set; }
        int AssignedTo { get; set; }
    }
}
