using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ComicShelf.Models;

namespace ComicShelf.Pages.SeriesNs
{
    public class EditModel : PageModel
    {
        private readonly ComicShelfContext _context;

        public EditModel(ComicShelfContext context)
        {
            _context = context;
            AvailablePublishers = _context.Publishers.OrderBy(x => x.Name).Select(x => new SelectListItem(x.Name, x.Id.ToString()));
            var enums = Enum.GetNames(typeof(Models.Enums.Type));
            var values = Enum.GetValues(typeof(Models.Enums.Type));
            for (var i = 0; i < values.Length; i++)
            {
                Types.Add(new SelectListItem { Text = enums[i], Value = values.GetValue(i).ToString() });
            }

        }

        [BindProperty]
        public SeriesModel Series { get; set; } = default!;
        public IEnumerable<SelectListItem> AvailablePublishers { get; set; }
        public List<SelectListItem> Types { get; set; } = new List<SelectListItem>();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Series == null)
            {
                return NotFound();
            }

            var series =  await _context.Series.Include(x=>x.Publishers).FirstOrDefaultAsync(m => m.Id == id);
            if (series == null)
            {
                return NotFound();
            }
            Series = new SeriesModel(series);
            foreach (var item in AvailablePublishers)
            {
                if(item.Value == Series.Id.ToString())
                    item.Selected = true;
            }
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
            
            var original = _context.Series.Include(x=>x.Publishers).Single(x => x.Id == Series.Id);

            Series.Update(_context, original);

            _context.Attach(original).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SeriesExists(Series.Id))
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

        private bool SeriesExists(int id)
        {
          return (_context.Series?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
