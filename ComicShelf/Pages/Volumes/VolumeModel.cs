using ComicShelf.Models.Enums;
using ComicShelf.Models;

namespace ComicShelf.Pages.Volumes
{
    public class VolumeModel
    {
        public int Number { get; set; }
        public required string Title { get; set; }
        public string Series { get; set; }
        public Status Status { get; set; }
        public Rating Raiting { get; set; }
        public PurchaseStatus PurchaseStatus { get; set; }

        public DateTime PurchaseDate { get; set; }
        //public string Cover { get; set; }
        public IFormFile CoverFile { get; set; }

        public string[] Authors { get; set; }
        public int Issues { get; set; }

        public VolumeModel()
        {
            PurchaseDate = DateTime.Today;
        }
    }
}
