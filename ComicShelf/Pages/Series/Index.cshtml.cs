using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ComicType = Backend.Models.Enums.Type;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.Services;
using ComicShelf.Utilities;
using Backend.Models;
using Services.ViewModels;

namespace ComicShelf.Pages.Series
{
    public class IndexModel : PageModel
    {
        private readonly SeriesService _service;
        private readonly EnumUtilities _enumUtilities;
        private readonly PublishersService _publishersService;

        public IndexModel(SeriesService service, EnumUtilities enumUtilities, PublishersService publishersService)
        {
            _service = service;
            _enumUtilities = enumUtilities;
            _publishersService = publishersService;

            Publishers = new SelectList(_publishersService.GetAll(), nameof(Publisher.Id), nameof(Publisher.Name));
        }

        public SelectList Publishers { get; set; }
        public IList<SeriesViewModel> Series { get; set; } = default!;

        public void OnGetAsync()
        {
            ViewData["Types"] = _enumUtilities.GetSelectItemList<ComicType>();
            ViewData["Publishers"] = Publishers;

            Series = _service.GetAll().ToList();
        }

        public IActionResult OnPostUpdate(SeriesUpdateModel series)
        {
            if (series is null)
                return BadRequest("Nothing to update");

            if (!_service.Exists(series.Id))
            {
                return NotFound(series);
            }

            _service.Update(series);

            var newSeries = _service.Get(series.Id);

            return Partial("_SeriesRowPartial", newSeries);
        }

        public IActionResult OnDelete(int id)
        {

            return StatusCode(200);
        }
    }
}
