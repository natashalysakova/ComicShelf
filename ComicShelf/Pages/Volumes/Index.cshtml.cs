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
    public class IndexModel : PageModel
    {
        private readonly ComicShelf.Models.ComicShelfContext _context;

        public IndexModel(ComicShelf.Models.ComicShelfContext context)
        {
            _context = context;
        }

        public IList<Volume> Volumes { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Volumes != null)
            {
                Volumes = await _context.Volumes.ToListAsync();
            }
        }
    }
}
