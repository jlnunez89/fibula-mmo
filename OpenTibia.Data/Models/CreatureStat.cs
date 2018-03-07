using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OpenTibia.Data.Contracts;

namespace OpenTibia.Data.Models
{
    [Table("creature_stat", Schema = "opentibia_classic")]
    public class CreatureStat : ICreatureStat
    {
        [Key]
        [Column(Order = 0)]
        public string Name { get; set; }
        public int KilledBy { get; set; }
        public int Killed { get; set; }
        [Key]
        [Column(Order = 1)]
        public long Time { get; set; }
    }
}
