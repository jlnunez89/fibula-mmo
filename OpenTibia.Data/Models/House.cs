using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OpenTibia.Data.Contracts;

namespace OpenTibia.Data.Models
{
    [Table("house", Schema = "opentibia_classic")]
    public class House : IAuction
    {
        [Key]
        public short HouseId { get; set; }
        public string HouseName { get; set; }
        public short RentOffset { get; set; }
        public short Area { get; set; }
        public byte GuildHouse { get; set; }
        public short Sqm { get; set; }
        public string Description { get; set; }
        public string Coords { get; set; }
        public int Price { get; set; }
        public int PriceOld { get; set; }
        public byte Auctioned { get; set; }
        public byte AuctionDays { get; set; }
        public int Bid { get; set; }
        public int BidderId { get; set; }
        public byte PricePerSqm { get; set; }
        public int Bidlimit { get; set; }
    }
}
