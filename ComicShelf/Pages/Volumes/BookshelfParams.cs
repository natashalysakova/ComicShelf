namespace ComicShelf.Pages.Volumes
{
    public class BookshelfParams
    {
        //public bool FilterAvailable { get; set; }
        //public bool FilterPreorder { get; set; }
        //public bool FilterWishlist { get; set; }
        //public bool FilterAnnounced { get; set; }
        //public bool FilterGone { get; set; }

        //Dictionary<string, bool> filters = new Dictionary<string, bool>();
        public string? filter { get; set; }
        public int? sort { get; set; }
        public string? direction { get; set; }

        public string? search { get; set; }

        public BookshelfParams()
        {
            direction = "up";
            sort = 2;

        }
    }
}
