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
using ComicShelf.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using Org.BouncyCastle.Crypto.Engines;
using NuGet.Packaging;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Localization;
using ComicShelf.Localization;
using ComicShelf.Utilities;

namespace ComicShelf.Pages.Volumes
{
    public class IndexModel : PageModel
    {
        private readonly VolumeService _volumeService;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly SearchService _searchService;
        public IndexModel(VolumeService volumeService, SearchService searchService, SeriesService seriesService, AuthorsService authorsService, IStringLocalizer<SharedResource> localizer)
        {
            _volumeService = volumeService;
            _localizer = localizer;
            _searchService = searchService;

            Statuses.AddRange(VolumeUtilities.GetStatusSelectItemList(_localizer));
            PurchaseStatuses.AddRange(VolumeUtilities.GetPurchaseStatusSelectItemList(_localizer));
            Ratings.AddRange(VolumeUtilities.GetRatings());
            Digitalities.AddRange(VolumeUtilities.GetDigitalitySelectItemList(_localizer));

            Authors.AddRange(authorsService.GetAll().Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }));
            Series.AddRange(seriesService.GetAll().Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }));

        }

        [BindProperty]
        public VolumeModel NewVolume { get; set; } = default!;

        public List<SelectListItem> Statuses { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> PurchaseStatuses { get; set; } = new List<SelectListItem>();
        public List<int> Ratings { get; set; } = new List<int>();
        public List<SelectListItem> Authors { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Series { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Digitalities { get; set; } = new List<SelectListItem>();

        public IList<Volume> Volumes { get; set; } = default!;
        public IList<Volume> AnnouncedAndPreordered
        {
            get
            {
                return Volumes.Where(x => x.PurchaseStatus == Models.Enums.PurchaseStatus.Announced || x.PurchaseStatus == Models.Enums.PurchaseStatus.Preordered).ToList();
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
            //Volumes = await volumeService.GetAll().Include(x => x.Series).OrderBy(x => x.Series.Name).ThenBy(x => x.Number).ToListAsync();
            Volumes = _volumeService.Filter(new BookshelfParams());
        }

        public PartialViewResult OnGetVolumeAsync(int id)
        {
            var volume = _volumeService.Get(id);
            _volumeService.LoadReference(volume, x => x.Series);
            _volumeService.LoadCollection(volume, x => x.Authors);

            var resultVD = new ViewDataDictionary<Volume>(ViewData, volume);
            resultVD["PurchaseStatuses"] = VolumeUtilities.GetPurchaseStatusesSelectItemList(volume, _localizer);
            resultVD["ReadingStatuses"] = VolumeUtilities.GetStatusSelectItemList(_localizer);
            resultVD["Ratings"] = VolumeUtilities.GetRatings();
            resultVD["Digitality"] = VolumeUtilities.GetDigitalitySelectItemList(_localizer);

            return new PartialViewResult()
            {
                ViewName = "_VolumePartial",
                ViewData = resultVD
            };
        }        

        public async Task<IActionResult> OnPostChangeStatus(VolumeUpdateModel volumeToUpdate)
        {
            _volumeService.UpdatePurchaseStatus(volumeToUpdate);
            var item = _volumeService.Get(volumeToUpdate.Id);
            if(item != null)
            {
                _volumeService.LoadReference(item, x => x.Series);
                var partial = Partial("_BookPartial", item);
                return partial;
            }

            return StatusCode(404);
        }

        public async Task<IActionResult> OnPostAddAsync()
        {
            if (!ModelState.IsValid || NewVolume == null)
            {
                return StatusCode(400, "Fill mandatory fields");
            }

            if (_volumeService.Exists(NewVolume.Series, NewVolume.Number))
            {
                return StatusCode(409, $"{NewVolume.Series} Volume #{NewVolume.Number} already exists");
            }

            _volumeService.Add(NewVolume);

            Volumes = _volumeService.Filter(new BookshelfParams());
            return Partial("_ShelfPartial", Volumes);
        }

        public PartialViewResult OnGetFiltered(BookshelfParams filters)
        {
            Volumes = _volumeService.Filter(filters);
            return Partial("_ShelfPartial", Volumes);
        }

    }
}
