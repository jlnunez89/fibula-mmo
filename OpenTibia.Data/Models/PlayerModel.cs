using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OpenTibia.Data.Contracts;

namespace OpenTibia.Data.Models
{
    [Table("players", Schema = "opentibia_classic")]
    public class PlayerModel : ICipPlayer
    {
        [Key]
        public short Player_Id { get; set; }
        public string Charname { get; set; }
        public int Account_Id { get; set; }
        public int Account_Nr { get; set; }
        public int Creation { get; set; }
        public int Lastlogin { get; set; }
        public byte Gender { get; set; }
        public byte Online { get; set; }
        public string Vocation { get; set; }
        public byte Hideprofile { get; set; }
        public int Playerdelete { get; set; }
        public short Level { get; set; }
        public string Residence { get; set; }
        public string Oldname { get; set; }
        public string Comment { get; set; }
        public string CharIp { get; set; }
    }
}
