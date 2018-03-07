namespace OpenTibia.Data.Contracts
{
    public interface IHouseTransfer
    {
        long Id { get; set; }
        short HouseId { get; set; }
        int TransferTo { get; set; }
        long Gold { get; set; }
        byte Done { get; set; }
    }
}
