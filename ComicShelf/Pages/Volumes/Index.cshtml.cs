using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ComicShelf.Models;
using ComicShelf.Services;
using Microsoft.Extensions.Localization;

namespace ComicShelf.Pages.Volumes
{
    public class IndexModel : PageModel
    {
        private readonly VolumeService volumeService;
        private readonly IStringLocalizer<IndexModel> _localizer;

        public IndexModel(VolumeService volumeService, IStringLocalizer<IndexModel> localizer)
        {
            this.volumeService = volumeService;
            this._localizer = localizer;

        }

        public IList<Volume> Volumes { get; set; } = default!;
        public IList<Volume> AnnouncedAndPreordered
        {
            get
            {
                return Volumes.Where(x=>x.PurchaseStatus == Models.Enums.PurchaseStatus.Announced || x.PurchaseStatus == Models.Enums.PurchaseStatus.Preordered).ToList();
            }
        }
        public IList<Volume> WishList
        {
            get
            {
                return Volumes.Where(x => x.PurchaseStatus == Models.Enums.PurchaseStatus.Wishlist).ToList();
            }
        }

        public IList<Volume> Purchased
        {
            get
            {
                return Volumes.Except(AnnouncedAndPreordered).Except(WishList).ToList();
            }
        }

        public async Task OnGetAsync()
        {
            Volumes = await volumeService.GetAll().Include(x => x.Series).OrderBy(x => x.Series.Name).ThenBy(x => x.Number).ToListAsync();
            ViewData["Title"] = _localizer.GetString("Main page");
        }

        public async Task<PartialViewResult> OnGetVolumeAsync(int id)
        {
            return Partial("_VolumePartial", volumeService.GetAll().Include(x=>x.Series).Single(x=>x.Id == id));
        }
    }
}
