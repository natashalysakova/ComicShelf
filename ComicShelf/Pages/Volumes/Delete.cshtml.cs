using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ComicShelf.Pages.Volumes
{
    public class DeleteModel : PageModel
    {
        private readonly ComicShelfContext _context;

        public DeleteModel(ComicShelfContext context)
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

            var volume = await _context.Volumes.FirstOrDefaultAsync(m => m.Id == id);

            if (volume == null)
            {
                return NotFound();
            }
            else 
            {
                Volume = volume;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Volumes == null)
            {
                return NotFound();
            }
            var volume = await _context.Volumes.FindAsync(id);

            if (volume != null)
            {
                Volume = volume;
                _context.Volumes.Remove(Volume);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
