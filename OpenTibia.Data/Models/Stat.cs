using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OpenTibia.Data.Contracts;

namespace OpenTibia.Data.Models
{
    [Table("stats", Schema = "opentibia_classic")]
    public class Stat : IStat
    {
        [Key]
        public int PlayersOnline { get; set; }

        public int RecordOnline { get; set; }
    }
}
