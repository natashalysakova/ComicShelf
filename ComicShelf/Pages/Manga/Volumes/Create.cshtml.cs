using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ComicShelf.Localization;
using Services.Services;
using Backend.Models.Enums;
using Services.ViewModels;
using ComicShelf.Utilities;

namespace ComicShelf.Pages.Volumes
{
    public class CreateModel : PageModel
    {
        private readonly VolumeService _volumeService;
        private readonly LocalizationService _localizer;
        private readonly EnumUtilities _enumUtilities;

        public CreateModel(VolumeService volumeService, SeriesService seriesService, AuthorsService authorsService, LocalizationService localizer, EnumUtilities enumUtilities)
        {
            _volumeService = volumeService;
            _localizer = localizer;
            _enumUtilities = enumUtilities;

            Statuses.AddRange(_enumUtilities.GetSelectItemList<Status>());
            PurchaseStatuses.AddRange(_enumUtilities.GetSelectItemList<PurchaseStatus>());
            Ratings.AddRange(_enumUtilities.GetRatingsSelectItemList());

            Authors.AddRange(authorsService.GetAll().Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }));
            Series.AddRange(seriesService.GetAll().Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }));
        }

        public List<SelectListItem> Statuses { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> PurchaseStatuses { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Ratings { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Authors { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Series { get; set; } = new List<SelectListItem>();


        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public VolumeCreateModel Volume { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || Volume == null)
            {
                return Page();
            }
            
            _volumeService.Add(Volume);
            return RedirectToPage("./Index");
        }
    }
}
