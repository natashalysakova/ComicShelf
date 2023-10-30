using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class Publisher : IIdEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int CountryId { get; set; }
        public virtual Country? Country { get; set; }
        public virtual ICollection<Series> Series { get; set; } = new List<Series>();


    }
}
