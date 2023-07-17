using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ComicShelf.Models;

namespace ComicShelf.Pages.SeriesNs
{
    public class DetailsModel : PageModel
    {
        private readonly ComicShelfContext _context;

        public DetailsModel(ComicShelfContext context)
        {
            _context = context;
        }

      public Models.Series Series { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Series == null)
            {
                return NotFound();
            }

            var series = await _context.Series.FirstOrDefaultAsync(m => m.Id == id);
            if (series == null)
            {
                return NotFound();
            }
            else 
            {
                Series = series;
            }
            return Page();
        }
    }
}
