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

namespace ComicShelf.Pages.Volumes
{
    public class EditModel : PageModel
    {
        VolumeService _volumeService;
        private readonly CoverService _coverService;

        public EditModel(VolumeService volumeService, CoverService coverService)
        {
            _volumeService = volumeService;
            _coverService = coverService;
            Statuses.AddRange(Utilities.GetEnumAsSelectItemList(typeof(Status)));
            PurchaseStatuses.AddRange(Utilities.GetEnumAsSelectItemList(typeof(PurchaseStatus)));
            Ratings.AddRange(Utilities.GetEnumAsSelectItemList(typeof(Rating)));

        }

        public List<SelectListItem> Statuses { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> PurchaseStatuses { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Ratings { get; set; } = new List<SelectListItem>();

        [BindProperty]
        public Volume Volume { get; set; } = default!;

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
            Volume = volume;
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
