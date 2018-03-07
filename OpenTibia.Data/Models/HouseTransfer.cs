using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OpenTibia.Data.Contracts;

namespace OpenTibia.Data.Models
{
    [Table("house_transfer", Schema = "opentibia_classic")]
    public class HouseTransfer : IHouseTransfer
    {
        [Key]
        public long Id { get; set; }
        public short HouseId { get; set; }
        public int TransferTo { get; set; }
        public long Gold { get; set; }
        public byte Done { get; set; }
    }
}
