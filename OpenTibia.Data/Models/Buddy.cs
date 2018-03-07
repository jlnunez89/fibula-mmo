using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OpenTibia.Data.Contracts;

namespace OpenTibia.Data.Models
{
    [Table("buddy", Schema = "opentibia_classic")]
    public class Buddy : IBuddy
    {
        [Key]
        public int EntryId { get; set; }
        public int AccountNr { get; set; }
        public int BuddyId { get; set; }
        public string BuddyString { get; set; }
        public long Timestamp { get; set; }
        public int InitiatingId { get; set; }
    }
}
