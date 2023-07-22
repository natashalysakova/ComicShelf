using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ComicShelf.Models;
using ComicShelf.Services;

namespace ComicShelf.Pages.SeriesNs
{
    public class EditModel : PageModel
    {
        private readonly SearchService _searchController;
        private readonly PublishersService _publishersController;
        private readonly SeriesService _seriesService;

        public EditModel(SearchService searchController, PublishersService publishersController, SeriesService seriesController)
        {
            _searchController = searchController;
            _seriesService = seriesController;

            AvailablePublishers = publishersController.GetAll().OrderBy(x => x.Name).Select(x => new SelectListItem(x.Name, x.Id.ToString()));
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
            if (id == null)
            {
                return NotFound();
            }

            var series = _seriesService.GetWithPublishers(id);
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
            //if (NewPublisher != null)
            //{
            //    var newPublisher = _context.Publishers.Add(new Publisher { Name = NewPublisher, Country = _context.UnknownCountry });
            //    _context.SaveChanges();
            //    Series.PublishersIds.Add(newPublisher.Entity.Id.ToString());
            //    ModelState.Clear();
            //    TryValidateModel(ModelState);
            //}

            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            try
            {
                _seriesService.Update(Series);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_seriesService.Exists(Series.Id))
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

        public IActionResult OnGetSearch(string term)
        {
            return new JsonResult(_searchController.FindPublishersByTerm(term));
        }
    }
}
