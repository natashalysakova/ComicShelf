using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using ComicShelf.Localization;
using ComicShelf.Models;
using ComicShelf.Models.Enums;
using ComicShelf.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using ComicShelf.Utilities;

namespace ComicShelf.Pages.SeriesNs
{
    public class CreateModel : PageModel
    {
        //private readonly Models.ComicShelfContext _context;
        private readonly SearchService _searchController;
        private readonly SeriesService _seriesService;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly EnumUtilities _utilities;

        public CreateModel(SearchService searchController, PublishersService publishersController, SeriesService seriesController, IStringLocalizer<SharedResource> localizer, EnumUtilities utilities)
        {
            _searchController = searchController;
            _seriesService = seriesController;
            _localizer = localizer;
            _utilities = utilities;

            AvailablePublishers = publishersController.GetAll().OrderBy(x => x.Name).Select(x => new SelectListItem(x.Name, x.Id.ToString()));

            Types.AddRange(_utilities.GetTypesSelectItemList());   
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public SeriesModel Series { get; set; } = default!;

        public IEnumerable<SelectListItem> AvailablePublishers { get; set; }
        public List<SelectListItem> Types { get; set; } = new List<SelectListItem>();


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public IActionResult OnPost()
        {
            //if (Publishers != null)
            //{
            //    var newPublisher = _context.Publishers.Add(new Publisher { Name = Publishers, Country = _context.Countries.Single(x => x.Name == "None") });
            //    Series.PublishersIds.Add(newPublisher.Entity.Id.ToString());
            //    ModelState.Clear();
            //    TryValidateModel(ModelState);
            //}

            if (!ModelState.IsValid ||  Series == null)
            {
                return Page();
            }

            _seriesService.Add(Series);

            return RedirectToPage("./Index");
        }

        public IActionResult OnGetSearch(string term)
        {
            return new JsonResult(_searchController.FindPublishersByTerm(term));
        }
    }
}
