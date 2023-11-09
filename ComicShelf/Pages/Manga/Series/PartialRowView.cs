using Microsoft.AspNetCore.Mvc.Rendering;
using Services.ViewModels;

namespace ComicShelf.Pages.Manga.Series
{
    public class PartialRowView
    {
        public SeriesUpdateModel UpdateItem { get; set; }
        public IEnumerable<SelectListItem> Types { get; set; }
        public SelectList Publishers { get; set; }
    }
}
