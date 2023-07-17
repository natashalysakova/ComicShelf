using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ComicShelf.Pages.SeriesNs
{
    public class CreateModel : PageModel
    {
        private readonly Models.ComicShelfContext _context;

        public CreateModel(Models.ComicShelfContext context)
        {
            _context = context;
            AvailablePublishers = _context.Publishers.OrderBy(x=>x.Name).Select(x => new SelectListItem(x.Name, x.Id.ToString()));
            var enums = Enum.GetNames(typeof(Models.Enums.Type));
            var values = Enum.GetValues(typeof(Models.Enums.Type));
            for ( var i = 0; i < values.Length; i++)
            {
                Types.Add(new SelectListItem { Text = enums[i], Value = values.GetValue(i).ToString() });
            }
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public SeriesModel Series { get; set; } = default!;

        public IEnumerable<SelectListItem> AvailablePublishers { get; set; }
        public List<SelectListItem> Types { get; set; } = new List<SelectListItem>();
        public IEnumerable<string> SelectedPublishers { get; set; } = Enumerable.Empty<string>();
        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.Series == null || Series == null)
            {
                return Page();
            }

            _context.Series.Add(Series.ToModel(_context));
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
