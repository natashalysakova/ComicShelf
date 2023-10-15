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
using SeriesM = ComicShelf.Models.Series;
using NuGet.Packaging;

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


            //Publishers.AddRange(_publishersService.GetAll().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }));

            Publishers = new SelectList(_publishersService.GetAll(), nameof(Publisher.Id), nameof(Publisher.Name));
        }

        public SelectList Publishers { get; set; }
        public IList<SeriesModel> Series { get; set; } = default!;

        public void OnGetAsync()
        {
            ViewData["Types"] = _enumUtilities.GetSelectItemList<ComicType>();
            ViewData["Publishers"] = Publishers;
            var tmp = _service.GetAll(); 
            foreach (var item in tmp) {
                if(item.Publisher != null)
                {
                    Console.WriteLine(item.Name);
                }
            }   
            Series = tmp.Select(x => new SeriesModel(x)).ToList() ;
        }

        public IActionResult OnPostUpdate(SeriesModel series)
        {
            if (series is null)
                return BadRequest("Nothing to update");

            if (!_service.Exists(series.Id))
            {
                return NotFound(series);
            }

            _service.Update(series);

            var newSeries = new SeriesModel(_service.Get(series.Id));

            return Partial("_SeriesRowPartial", newSeries);
        }

        public IActionResult OnDelete(int id)
        {

            return StatusCode(200);
        }
    }
}
