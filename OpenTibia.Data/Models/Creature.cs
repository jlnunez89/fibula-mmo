using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OpenTibia.Data.Contracts;

namespace OpenTibia.Data.Models
{
    [Table("creatures", Schema = "opentibia_classic")]
    public class CipCreature : ICipCreature
    {
        [Key]
        public byte Id { get; set; }
        public string Race { get; set; }
        public string Plural { get; set; }
        public string Description { get; set; }
    }
}
