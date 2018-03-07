using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OpenTibia.Data.Contracts;

namespace OpenTibia.Data.Models
{
    [Table("online", Schema = "opentibia_classic")]
    public class OnlinePlayer : IOnlinePlayer
    {
        [Key]
        public string Name { get; set; }
        public int Level { get; set; }
        public string Vocation { get; set; }
    }   
}
