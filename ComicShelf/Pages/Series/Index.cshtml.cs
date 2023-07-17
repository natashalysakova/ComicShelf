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
    public class IndexModel : PageModel
    {
        private readonly ComicShelfContext _context;

        public IndexModel(ComicShelfContext context)
        {
            _context = context;
        }

        public IList<Models.Series> Series { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Series != null)
            {
                Series = await _context.Series.OrderBy(x=>x.Name).Include("Publishers").Include("Publishers.Country").ToListAsync();
            }
        }
    }
}
