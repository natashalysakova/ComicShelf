using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
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

        public IList<PublisherViewModel> Publisher { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Publisher = _publisherService.GetAll().OrderByDescending(x=>x.SeriesCount).ThenBy(x=>x.Name).ToList();
        }
    }
}
