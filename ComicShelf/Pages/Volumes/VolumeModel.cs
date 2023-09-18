using ComicShelf.Models.Enums;
using ComicShelf.Models;
using System.ComponentModel.DataAnnotations;

namespace ComicShelf.Pages.Volumes
{
    public class VolumeModel
    {
        public int Number { get; set; }

        [Required]
        public string Title { get; set; }
        public string Series { get; set; }
        public Status Status { get; set; }
        public Rating Raiting { get; set; }
        public PurchaseStatus PurchaseStatus { get; set; }

        public DateTime PurchaseDate { get; set; }
        //public string Cover { get; set; }
        public IFormFile CoverFile { get; set; }

        public string[] Authors { get; set; }
        public int Issues { get; set; }
    }
}
