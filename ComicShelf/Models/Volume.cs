using ComicShelf.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComicShelf.Models
{
    public class Volume 
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Number { get; set; }
        public required string Title { get; set; }
        public Series Series { get; set; }
        public Status Status { get; set; }
        public Rating Raiting { get; set; }
        public PurchaseStatus PurchaseStatus { get; set; }
        public DateTime PurchaseDate { get; set; }
        public virtual ICollection<Author> Authors { get; set; } = new List<Author>();
        public virtual ICollection<Issue> Issues { get; set; } = new List<Issue>();
    }
}
