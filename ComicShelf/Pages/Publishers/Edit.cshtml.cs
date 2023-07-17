using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ComicShelf.Models;

namespace ComicShelf.Pages.Publishers
{
    public class EditModel : PageModel
    {
        private readonly ComicShelfContext _context;

        public EditModel(ComicShelfContext context)
        {
            _context = context;
            CountriesList = _context.Countries.Select(x => new SelectListItem(x.Name, x.Id.ToString()));

        }

        [BindProperty]
        public PublisherModel Publisher { get; set; } = default!;

        public IEnumerable<SelectListItem> CountriesList { get; set; }


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Publishers == null)
            {
                return NotFound();
            }

            var publisher =  await _context.Publishers.Include(x=>x.Country).FirstOrDefaultAsync(m => m.Id == id);
            if (publisher == null)
            {
                return NotFound();
            }
            Publisher = new PublisherModel(publisher);
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

            _context.Attach(Publisher.ToModel(_context)).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PublisherExists(Publisher.Id))
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

        private bool PublisherExists(int id)
        {
          return (_context.Publishers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
