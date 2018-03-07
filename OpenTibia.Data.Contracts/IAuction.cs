namespace OpenTibia.Data.Contracts
{
    public interface IAuction
    {
        short HouseId { get; set; }
        string HouseName { get; set; }
        short RentOffset { get; set; }
        short Area { get; set; }
        byte GuildHouse { get; set; }
        short Sqm { get; set; }
        string Description { get; set; }
        string Coords { get; set; }
        int Price { get; set; }
        int PriceOld { get; set; }
        byte Auctioned { get; set; }
        byte AuctionDays { get; set; }
        int Bid { get; set; }
        int BidderId { get; set; }
        byte PricePerSqm { get; set; }
        int Bidlimit { get; set; }
    }
}
