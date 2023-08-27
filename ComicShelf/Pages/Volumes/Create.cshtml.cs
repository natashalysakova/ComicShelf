using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ComicShelf.Models;
using ComicShelf.Services;
using ComicShelf.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace ComicShelf.Pages.Volumes
{
    public class CreateModel : PageModel
    {
        private readonly VolumeService _volumeService;
        private readonly SearchService _searchService;
        private readonly SeriesService _seriesService;

        public CreateModel(VolumeService volumeService, SearchService searchService, SeriesService seriesService)
        {
            _volumeService = volumeService;
            _searchService = searchService;
            _seriesService = seriesService;

            Statuses.AddRange(Utilities.GetEnumAsSelectItemList(typeof(Status)));
            PurchaseStatuses.AddRange(Utilities.GetEnumAsSelectItemList(typeof(PurchaseStatus)));
            Ratings.AddRange(Utilities.GetEnumAsSelectItemList(typeof(Rating)));
        }

        public List<SelectListItem> Statuses { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> PurchaseStatuses { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Ratings { get; set; } = new List<SelectListItem>();
        

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Volume Volume { get; set; } = default!;
        [BindProperty]
        [Required]
        public string SelectedSeries { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            var series = _seriesService.GetByName(SelectedSeries);
            if (series != null)
            {
                Volume.Series = series;
            }

            if (!ModelState.IsValid || Volume == null)
            {
                return Page();
            }
            
            _volumeService.Add(Volume);
            return RedirectToPage("./Index");
        }

        public IActionResult OnGetSearch(string term)
        {
            var res = _searchService.FindSeriesByTerm(term);
            return new JsonResult(res);
        }
    }
}
