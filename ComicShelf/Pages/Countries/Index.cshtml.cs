using Backend.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Services.Services;
using Services.ViewModels;

namespace ComicShelf.Pages.Countries
{
    public class IndexModel : PageModel
    {
        private readonly CountryService _service;

        public IndexModel(CountryService service)
        {
            _service = service;
        }

        public List<CountryViewModel> Countries { get;set; } = default!;

        public void OnGet()
        {
            Countries = _service.GetAll().Where(x => x.Publishers.Any()).ToList();
        }
    }
}
