using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Services;
using Services.ViewModels;

namespace ComicShelf.Pages.Publishers
{
    public class DetailsModel : PageModel
    {
        private readonly PublishersService _publisherService;

        public DetailsModel(PublishersService service)
        {
            _publisherService = service;
        }

      public PublisherViewModel Publisher { get; set; } = default!; 

        public IActionResult OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publisher = _publisherService.GetWithCountry(id);
            if (publisher == null)
            {
                return NotFound();
            }
            else 
            {
                Publisher = publisher;
            }
            return Page();
        }
    }
}
