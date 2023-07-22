using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ComicShelf.Models;
using ComicShelf.Services;
using System.ComponentModel.DataAnnotations;

namespace ComicShelf.Pages.Publishers
{
    public class EditModel : PageModel
    {
        private readonly PublishersService _publisherService;
        private readonly CountryService _countryService;

        public EditModel(PublishersService service, CountryService countryService)
        {
            _publisherService = service;
            _countryService = countryService;
            CountriesList = _countryService.GetCountriesForView();

        }

        [BindProperty]
        public Publisher Publisher { get; set; } = default!;

        public IEnumerable<SelectListItem> CountriesList { get; set; }

        [BindProperty]
        [Required]
        public string SelectedCountry { get; set; }


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
            Publisher = publisher;
            SelectedCountry = publisher.Country.Id.ToString();
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var country = _countryService.Get(int.Parse(SelectedCountry));
            if (country != null)
            {
                Publisher.Country = country;
            }

            try
            {
                _publisherService.Update(Publisher);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_publisherService.Exists(Publisher.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

    }
}
