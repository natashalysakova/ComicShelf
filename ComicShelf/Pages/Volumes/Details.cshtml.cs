using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ComicShelf.Models;

namespace ComicShelf.Pages.Volumes
{
    public class DetailsModel : PageModel
    {
        private readonly ComicShelf.Models.ComicShelfContext _context;

        public DetailsModel(ComicShelf.Models.ComicShelfContext context)
        {
            _context = context;
        }

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
    }
}
