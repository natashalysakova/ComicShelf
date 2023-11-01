using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Services;
using Services.ViewModels;

namespace ComicShelf.Pages.Publishers
{
    public class IndexModel : PageModel
    {
        private readonly PublishersService _publisherService;

        public IndexModel(PublishersService service)
        {
            _publisherService = service;
        }

        public IEnumerable<PublisherViewModel> Publisher { get; set; } = default!;

        public void OnGetAsync()
        {
            var t = _publisherService.GetAll();

            Publisher = t.OrderBy(x => x.Name).Select(x => { x.Series = x.Series.OrderBy(y => y.Name); return x; });
        }
    }
}
