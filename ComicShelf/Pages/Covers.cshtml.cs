using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ComicShelf.Models;

namespace ComicShelf.Pages
{
    public class CoversModel : PageModel
    {
        private readonly ComicShelf.Models.ComicShelfContext _context;

        public CoversModel(ComicShelf.Models.ComicShelfContext context)
        {
            _context = context;
        }

        public IList<VolumeCover> VolumeCover { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.VolumeCovers != null)
            {
                VolumeCover = await _context.VolumeCovers
                .Include(v => v.Volume).Include(x=>x.Volume.Series).ToListAsync();
            }
        }
    }
}
