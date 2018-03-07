using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OpenTibia.Data.Contracts;

namespace OpenTibia.Data.Models
{
    [Table("deaths", Schema = "opentibia_classic")]
    public class Death : IDeath
    {
        [Key]
        public int RecordId { get; set; }
        public int PlayerId { get; set; }
        public int Level { get; set; }
        public byte ByPeekay { get; set; }
        public int PeekayId { get; set; }
        public string CreatureString { get; set; }
        public byte Unjust { get; set; }
        public long Timestamp { get; set; }
    }
}
