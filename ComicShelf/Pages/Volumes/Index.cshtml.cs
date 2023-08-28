using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ComicShelf.Models;
using ComicShelf.Services;

namespace ComicShelf.Pages.Volumes
{
    public class IndexModel : PageModel
    {
        private readonly VolumeService volumeService;

        public IndexModel(VolumeService volumeService)
        {
            this.volumeService = volumeService;
        }

        public IList<Volume> Volumes { get; set; } = default!;
        public IList<Volume> AnnouncedAndPreordered
        {
            get
            {
                return Volumes.Where(x=>x.PurchaseStatus == Models.Enums.PurchaseStatus.Announced || x.PurchaseStatus == Models.Enums.PurchaseStatus.Preordered).ToList();
            }
        }

        public IList<Volume> Purchased
        {
            get
            {
                return Volumes.Except(AnnouncedAndPreordered).ToList();
            }
        }

        public async Task OnGetAsync()
        {
            Volumes = await volumeService.GetAll().Include(x => x.Series).OrderBy(x => x.Series.Name).ThenBy(x => x.Number).ToListAsync();

        }

        public async Task<PartialViewResult> OnGetVolumeAsync(int id)
        {
            return Partial("_VolumePartial", volumeService.GetAll().Include(x=>x.Series).Single(x=>x.Id == id));
        }
    }
}
