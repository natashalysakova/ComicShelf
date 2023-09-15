using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using ComicShelf.Models;
using ComicShelf.Models.Enums;
using ComicShelf.Pages.Volumes;
using ComicShelf.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ComicShelf.Pages.SeriesNs
{
    public class CreateModel : PageModel
    {
        //private readonly Models.ComicShelfContext _context;
        private readonly SearchService _searchController;
        private readonly SeriesService _seriesService;

        public CreateModel(SearchService searchController, PublishersService publishersController, SeriesService seriesController)
        {
            _searchController = searchController;
            _seriesService = seriesController;

            AvailablePublishers = publishersController.GetAll().OrderBy(x => x.Name).Select(x => new SelectListItem(x.Name, x.Id.ToString()));

            Types.AddRange(Utilities.GetEnumAsSelectItemList(typeof(Models.Enums.Type)));   
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
