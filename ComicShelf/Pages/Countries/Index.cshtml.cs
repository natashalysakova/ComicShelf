using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ComicShelf.Models;

namespace ComicShelf.Pages.Countries
{
    public class IndexModel : PageModel
    {
        private readonly ComicShelfContext _context;

        public IndexModel(ComicShelfContext context)
        {
            _context = context;
        }

        public IList<Country> Country { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Countries != null)
            {
                Country = await _context.Countries.OrderBy(x=>x.Name).ToListAsync();
            }
        }
    }
}
