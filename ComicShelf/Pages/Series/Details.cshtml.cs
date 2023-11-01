using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Services;
using Services.ViewModels;

namespace ComicShelf.Pages.SeriesNs
{
    public class DetailsModel : PageModel
    {
        private readonly SeriesService _seriesService;

        public DetailsModel(SeriesService seriesService)
        {
            _seriesService = seriesService;
        }

        public SeriesViewModel Series { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var series = _seriesService.Get(id);
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
