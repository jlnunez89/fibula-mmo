using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OpenTibia.Data.Contracts;

namespace OpenTibia.Data.Models
{
    [Table("rand_players", Schema = "opentibia_classic")]
    public class RandPlayer : IRandPlayer
    {
        [Key]
        public int RandId { get; set; }
        public int AccountId { get; set; }
        public int Order { get; set; }
        public int AssignedTo { get; set; }
    }
}
