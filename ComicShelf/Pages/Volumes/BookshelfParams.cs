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

        public BookshelfParams()
        {
            direction = DirectionEnum.up;
            sort = SortEnum.ByPurchaseDate;
            digitality = DigitalityEnum.All;
        }

        internal VolumeType ConvertDigitality()
        {
            return (VolumeType)((int)digitality & 0x01);
        }
    }

    public enum DigitalityEnum
    {

        Physical = 1,
        Digital = 2,
        All = 0
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
        ByCreationDate = 0,
        BySeriesTitle = 1,
        ByPurchaseDate = 2
    }
}
