using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ComicShelf.Models;

namespace ComicShelf.Pages.Publishers
{
    public partial class CreateModel : PageModel
    {
        private readonly ComicShelfContext _context;

        public CreateModel(ComicShelfContext context)
        {
            _context = context;
            CountriesList = _context.Countries.Select(x => new SelectListItem(x.Name, x.Id.ToString()));
        }

        public IActionResult OnGet()
        {

            return Page();
        }

        [BindProperty]
        public PublisherModel Publisher { get; set; }
        public IEnumerable<SelectListItem> CountriesList { get; set; }




        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || _context.Publishers == null || Publisher == null)
            {
                return Page();
            }

            _context.Publishers.Add(Publisher.ToModel(_context)) ;

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
