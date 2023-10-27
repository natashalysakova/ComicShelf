using ComicShelf.Models;
using ComicShelf.Models.Enums;

namespace ComicShelf.Pages.Volumes
{
    public class BookshelfParams
    {
        public PurchaseFilterEnum filter { get; set; }
        public SortEnum sort { get; set; }
        public DirectionEnum direction { get; set; }

        public string? search { get; set; }
        public DigitalityEnum digitality { get; set; }
        public ReadingEnum reading { get; set; }

    }

    public enum DigitalityEnum
    {
        All = 0,
        Physical = 1,
        Digital = 2
    }

    public enum DirectionEnum
    {
        up = 0,
        down = 1
    }

    public enum PurchaseFilterEnum
    {
        All = 0,
        Available = 1,
        Preorders = 2,
        Wishlist = 3,
        Announced = 4,
        Gone = 5
    }

    public enum SortEnum
    {
        ByCreationDate = 2,
        BySeriesTitle = 1,
        ByPurchaseDate = 0
    }

    public enum ReadingEnum {
        All = 0,
        NotStarted = 1,
        InQueue = 2,
        Reading = 3,
        Completed = 4,
        Dropped = 5,
        NewSeries = 6,

    }
}
