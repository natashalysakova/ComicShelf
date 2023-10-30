using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class Country : IIdEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string FlagPNG { get; set; }
        [Required]
        public string FlagSVG { get; set; }
        [Required]
        public  string CountryCode { get; set; }   

        public virtual ICollection<Publisher> Publishers { get; set; } = new List<Publisher>();
    }
    
}
