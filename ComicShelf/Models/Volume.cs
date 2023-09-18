using ComicShelf.Models.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComicShelf.Models
{
    public class Volume 
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Number { get; set; }

        [Required]
        public string Title { get; set; }

        public Status Status { get; set; }
        public Rating Raiting { get; set; }
        public PurchaseStatus PurchaseStatus { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string CoverUrl { get; set; }
        public DateTime CreationDate { get; set; }

        public int SeriesId { get; set; }
        public virtual Series Series { get; set; }

        //public int CoverId { get; set; }
        //public virtual VolumeCover Cover { get; set; }

        public virtual ICollection<Author> Authors { get; set; } = new List<Author>();
        public virtual ICollection<Issue> Issues { get; set; } = new List<Issue>();
    }
}
