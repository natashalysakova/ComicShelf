using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Services;
using Services.ViewModels;

namespace ComicShelf.Pages.Series
{
    public class DeleteModel : PageModel
    {
        private readonly SeriesService _seriesService;

        public DeleteModel(SeriesService seriesService)
        {
            _seriesService = seriesService;
        }

        [BindProperty]
        public SeriesViewModel Series { get; set; } = default!;

        public IActionResult OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var series = _seriesService.Get(id.Value);

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

        public IActionResult OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var series = _seriesService.Get(id.Value);

            if (series != null)
            {
                Series = series;
                _seriesService.Remove(id);
            }

            return RedirectToPage("./Index");
        }
    }
}
