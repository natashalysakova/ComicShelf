using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Services.Services;
using Services.ViewModels;
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
        public PublisherUpdateModel Publisher { get; set; } = default!;

        public IEnumerable<SelectListItem> CountriesList { get; set; }

        [BindProperty]
        [Required]
        public int SelectedCountry { get; set; }


        public IActionResult OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publisher = _publisherService.Get(id);
            if (publisher == null)
            {
                return NotFound();
            }
            //Publisher = publisher;
            SelectedCountry = publisher.CountryId;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public IActionResult OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var country = _countryService.Get(SelectedCountry);
            if (country != null)
            {
                Publisher.CountryId = country.Id;
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
