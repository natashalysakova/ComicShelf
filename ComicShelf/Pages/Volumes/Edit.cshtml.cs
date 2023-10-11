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
using ComicShelf.Models.Enums;
using Microsoft.Extensions.Localization;
using ComicShelf.Localization;
using ComicShelf.Utilities;

namespace ComicShelf.Pages.Volumes
{
    public class EditModel : PageModel
    {
        VolumeService _volumeService;
        private readonly AuthorsService _authorsService;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly EnumUtilities _enumUtilities;

        public EditModel(VolumeService volumeService, AuthorsService authorsService, IStringLocalizer<SharedResource> localizer, EnumUtilities enumUtilities)
        {
            _volumeService = volumeService;
            _authorsService = authorsService;
            _localizer = localizer;
            _enumUtilities = enumUtilities;

            Statuses.AddRange(_enumUtilities.GetStatusSelectItemList());
            PurchaseStatuses.AddRange(_enumUtilities.GetPurchaseStatusSelectItemList());
            Ratings.AddRange(_enumUtilities.GetRatings());

        }

        public List<SelectListItem> Statuses { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> PurchaseStatuses { get; set; } = new List<SelectListItem>();
        public List<int> Ratings { get; set; } = new List<int>();
        public List<SelectListItem> Authors { get; set; } = new List<SelectListItem>();

        [BindProperty]
        public Volume Volume { get; set; } = default!;
        [BindProperty]
        public IList<string> SelectedAutors { get; set; }


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var volume = _volumeService.Get(id);
            if (volume == null)
            {
                return NotFound();
            }
            _volumeService.LoadReference(volume, x => x.Series);
            _volumeService.LoadCollection(volume, x => x.Authors);

            Volume = volume;
            Authors.AddRange(_authorsService.GetAll().Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }));
            SelectedAutors = volume.Authors.Select(x=>x.Id.ToString()).ToList();

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
         
            try
            {
                Volume.Authors = _authorsService.GetAll().Where(x=> SelectedAutors.Contains(x.Id.ToString())).ToList();

                _volumeService.Update(Volume);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_volumeService.Exists(Volume.Id))
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
    }
}
