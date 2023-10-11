using ComicShelf.Models.Enums;

namespace ComicShelf.Pages.Volumes
{
    public class VolumeUpdateModel
    {
        public int Id { get; set; }
        public PurchaseStatus PurchaseStatus { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime ReleaseDate { get; set; }
        public Status Status { get; set; }
        public int Rating { get; set; }
        public IFormFile? CoverFile { get; set; }

    }
}
