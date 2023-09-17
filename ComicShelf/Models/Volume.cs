using ComicShelf.Models.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComicShelf.Models
{
    public class Volume 
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Number { get; set; }
        public required string Title { get; set; }
        public virtual Series? Series { get; set; }
        public Status Status { get; set; }
        public Rating Raiting { get; set; }
        public PurchaseStatus PurchaseStatus { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string CoverUrl { get; set; }
        public virtual VolumeCover? Cover { get; set; }
        public DateTime CreationDate { get; set; }

        public virtual ICollection<Author> Authors { get; set; } = new List<Author>();
        public virtual ICollection<Issue> Issues { get; set; } = new List<Issue>();
    }

    public class VolumeCover
    {
        [ForeignKey("Volume")]
        public int Id { get; set; }
        public virtual Volume Volume { get; private set; }
        public byte[] Cover { get; set; }
        public string Extention { get; set; }
    }
}
