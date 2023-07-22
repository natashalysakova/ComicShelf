using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ComicShelf.Models;
using ComicShelf.Services;
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
        public Publisher Publisher { get; set; }

        [BindProperty]
        [Required]
        public string SelectedCountry { get; set; }
        public IEnumerable<SelectListItem> CountriesList { get; set; }




        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            var country = _countryService.Get(int.Parse(SelectedCountry));
            if (country != null)
            {
                Publisher.Country = country;
            }

            if (!ModelState.IsValid || Publisher == null)
            {
                return Page();
            }

            _publisherService.Add(Publisher);

            return RedirectToPage("./Index");
        }
    }
}
