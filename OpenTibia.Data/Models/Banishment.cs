using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OpenTibia.Data.Contracts;

namespace OpenTibia.Data.Models
{
    [Table("banishments", Schema = "opentibia_classic")]
    public class Banishment : IBanishment
    {
        [Key]
        public short BanishmentId { get; set; }
        public int AccountNr { get; set; }
        public int AccountId { get; set; }
        public string Ip { get; set; }
        public string Violation { get; set; }
        public string Comment { get; set; }
        public int Timestamp { get; set; }
        public int BanishedUntil { get; set; }
        public int GmId { get; set; }
        public byte PunishmentType { get; set; }
    }
}
