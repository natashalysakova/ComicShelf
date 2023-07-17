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
    public class IndexModel : PageModel
    {
        private readonly ComicShelfContext _context;

        public IndexModel(ComicShelfContext context)
        {
            _context = context;
        }

        public IList<Publisher> Publisher { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Publishers != null)
            {
                Publisher = await _context.Publishers.OrderBy(x=>x.Name).Include(x=>x.Country).Include(x=>x.Series).ToListAsync();
            }
        }
    }
}
