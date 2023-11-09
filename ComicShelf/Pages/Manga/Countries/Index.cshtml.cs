using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Services;
using Services.ViewModels;

namespace ComicShelf.Pages.Manga.Countries
{
    public class IndexModel : PageModel
    {
        private readonly CountryService _service;

        public IndexModel(CountryService service)
        {
            _service = service;
        }

        public List<CountryViewModel> Countries { get; set; } = default!;

        public void OnGet()
        {
            Countries = _service.GetAll()
                .Where(x => x.Publishers.Any())
                .OrderBy(x => x.Publishers.Count())
                .Select(x => { x.Publishers = x.Publishers.OrderBy(y => y.Name); return x; })
                .ToList();
        }
    }
}
