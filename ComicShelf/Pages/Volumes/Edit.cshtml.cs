using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ComicShelf.Models;

namespace ComicShelf.Pages.Volumes
{
    public class EditModel : PageModel
    {
        private readonly ComicShelf.Models.ComicShelfContext _context;

        public EditModel(ComicShelf.Models.ComicShelfContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Volume Volume { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Volumes == null)
            {
                return NotFound();
            }

            var volume =  await _context.Volumes.FirstOrDefaultAsync(m => m.Id == id);
            if (volume == null)
            {
                return NotFound();
            }
            Volume = volume;
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

            _context.Attach(Volume).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VolumeExists(Volume.Id))
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

        private bool VolumeExists(int id)
        {
          return (_context.Volumes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
