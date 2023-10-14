using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ComicShelf.Models;
using ComicShelf.Services;
using ComicShelf.Utilities;
using ComicType = ComicShelf.Models.Enums.Type;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ComicShelf.Pages.SeriesNs
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
            Publishers.AddRange(_publishersService.GetAll().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }));
        }

        public List<SelectListItem> Publishers { get; set; } = new List<SelectListItem>();
        public IList<Models.Series> Series { get; set; } = default!;

        public void OnGetAsync()
        {
            ViewData["Types"] = _enumUtilities.GetSelectItemList<ComicType>();

            Series = _service.GetAll().Include("Publishers").Include("Publishers.Country").Include("Volumes").ToList();
        }

        public IActionResult OnPostUpdate(Models.Series series)
        {
            if (!_service.Exists(series.Id))
            {
                return NotFound(series);
            }

            _service.Update(series);

            return StatusCode(200);
        }
    }
}
