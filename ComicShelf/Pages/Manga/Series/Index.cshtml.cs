using ComicShelf.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.Services;
using Services.ViewModels;
using ComicType = Backend.Models.Enums.Type;

namespace ComicShelf.Pages.Manga.Series
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

            var p = _publishersService.GetAll().OrderBy(x=>x.Name);

            Publishers = new SelectList(p, nameof(PublisherViewModel.Id), nameof(PublisherViewModel.Name));

            Types = _enumUtilities.GetSelectItemList<ComicType>();
        }

        public SelectList Publishers { get; set; }
        public IList<SeriesUpdateModel> Series { get; set; } = default!;
        public IEnumerable<SelectListItem> Types { get; set; }

        public SeriesUpdateModel UpdateModel { get; set; }


        public void OnGetAsync()
        {
            Series = _service.GetAllForUpdate().OrderBy(x => x.Name).ToList();
        }

        public IActionResult OnPostUpdate(SeriesUpdateModel UpdateItem)
        {
            if (UpdateItem is null)
                return BadRequest("Nothing to update");

            if (!_service.Exists(UpdateItem.Id))
            {
                return NotFound(UpdateItem);
            }

            _service.Update(UpdateItem);

            var newSeries = _service.GetForUpdate(UpdateItem.Id);

            return Partial("_SeriesRowPartial", new PartialRowView() { Publishers = Publishers, Types = Types, UpdateItem = newSeries });
        }

        public IActionResult OnDelete(int id)
        {

            return StatusCode(200);
        }
    }
}
