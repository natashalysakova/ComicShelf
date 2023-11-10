using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Principal;

namespace Backend.Models
{
    public class Issue : IIdEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }

        public int VolumeId { get; set; }
        public virtual Volume Volume { get; set; }
    }

    public class Bonus : Issue
    {

    }
}
