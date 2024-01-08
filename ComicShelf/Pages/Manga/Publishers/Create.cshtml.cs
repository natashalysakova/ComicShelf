using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.Services;
using Services.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace ComicShelf.Pages.Publishers
{
    public partial class CreateModel : PageModel
    {
        private readonly PublishersService _publisherService;
        private readonly CountryService _countryService;


        public CreateModel(PublishersService publishersService, CountryService countryService)
        {
            _publisherService = publishersService;
            _countryService = countryService;


            CountriesList = _countryService.GetCountriesForView();
        }



        public IActionResult OnGet()
        {

            return Page();
        }

        [BindProperty]
        public PublisherCreateModel Publisher { get; set; }
        public IEnumerable<SelectListItem> CountriesList { get; set; }




        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public IActionResult OnPostAsync()
        {
            if (!ModelState.IsValid || Publisher == null)
            {
                return Page();
            }

            _publisherService.Add(Publisher);

            return RedirectToPage("./Index");
        }
    }
}
