using System.ComponentModel.DataAnnotations.Schema;

namespace ComicShelf.Models
{
    public class Country
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public required string Name { get; set; }
        public required string Flag { get; set; }

        public virtual ICollection<Publisher> Publishers { get; set; } = new List<Publisher>();
    }
    
}
