using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ComicShelf.Models;

namespace ComicShelf.Pages.Publishers
{
    public class DeleteModel : PageModel
    {
        private readonly ComicShelfContext _context;

        public DeleteModel(ComicShelfContext context)
        {
            _context = context;
        }

        [BindProperty]
      public Publisher Publisher { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Publishers == null)
            {
                return NotFound();
            }

            var publisher = await _context.Publishers.FirstOrDefaultAsync(m => m.Id == id);

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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Publishers == null)
            {
                return NotFound();
            }
            var publisher = await _context.Publishers.FindAsync(id);

            if (publisher != null)
            {
                Publisher = publisher;
                _context.Publishers.Remove(Publisher);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
