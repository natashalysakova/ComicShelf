using System.ComponentModel.DataAnnotations.Schema;

namespace ComicShelf.Models
{
    public class Publisher
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public required string Name { get; set; }
        public required Country Country { get; set; }
        public virtual ICollection<Series> Series { get; set; } = new List<Series>();


    }
}
