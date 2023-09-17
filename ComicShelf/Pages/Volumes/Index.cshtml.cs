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
using NuGet.Packaging;

namespace ComicShelf.Pages.Volumes
{
    public class IndexModel : PageModel
    {
        private readonly VolumeService volumeService;
        private readonly IStringLocalizer<IndexModel> _localizer;
        private readonly SearchService _searchService;

        public IndexModel(VolumeService volumeService, SearchService searchService, SeriesService seriesService, AuthorsService authorsService, IStringLocalizer<IndexModel> localizer)
        {
            this.volumeService = volumeService;
            this._localizer = localizer;

            _searchService = searchService;
            Statuses.AddRange(Utilities.GetEnumAsSelectItemList(typeof(Status)));
            PurchaseStatuses.AddRange(Utilities.GetEnumAsSelectItemList(typeof(PurchaseStatus)));
            Ratings.AddRange(Utilities.GetEnumAsSelectItemList(typeof(Rating)));
            Authors.AddRange(authorsService.GetAll().Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }));
            Series.AddRange(seriesService.GetAll().Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }));

        }

        [BindProperty]
        public VolumeModel NewVolume { get; set; } = default!;

        public List<SelectListItem> Statuses { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> PurchaseStatuses { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Ratings { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Authors { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Series { get; set; } = new List<SelectListItem>();

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
            //Volumes = await volumeService.GetAll().Include(x => x.Series).OrderBy(x => x.Series.Name).ThenBy(x => x.Number).ToListAsync();
            Volumes = await volumeService.GetAll().OrderByDescending(x => x.CreationDate).ToListAsync();
            ViewData["Title"] = _localizer.GetString("Main page");
        }

        public PartialViewResult OnGetVolumeAsync(int id)
        {
            return Partial("_VolumePartial", volumeService.GetAll().Single(x=>x.Id == id));
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || NewVolume == null)
            {
                return StatusCode(400, "Fill mandatory fields");
            }

            if (volumeService.Exists(NewVolume.Series, NewVolume.Number))
            {
                return StatusCode(409, $"{NewVolume.Series} Volume #{NewVolume.Number} already exists");
            }

            volumeService.Add(NewVolume);

            Volumes = await volumeService.GetAll().OrderByDescending(x => x.CreationDate).ToListAsync();
            return Partial("_ShelfPartial", Volumes);
        }

        public PartialViewResult OnGetFiltered(PurchaseFilters purchaseFilters)
        {
            Volumes = volumeService.Filter(purchaseFilters);
            return Partial("_ShelfPartial", Volumes);
        }

        //public IActionResult OnGetSearchSeries(string term)
        //{
        //    var res = _searchService.FindSeriesByTerm(term);
        //    return new JsonResult(res);
        //}
        //public IActionResult OnGetSearchAutors(string term)
        //{
        //    var res = _searchService.FindAutorByTerm(term);
        //    return new JsonResult(res);
        //}
    }

    public class PurchaseFilters
    {
        public bool FilterAvailable { get; set; }
        public bool FilterPreorder { get; set; }
        public bool FilterWishlist { get; set; }
        public bool FilterAnnounced { get; set; }
        public bool FilterGone { get; set; }

        internal IEnumerable<PurchaseStatus> ToStatusList()
        {
            IList<PurchaseStatus> statusList = new List<PurchaseStatus>();

            if(FilterAvailable)
            {
                statusList.Add(PurchaseStatus.Bought);
                statusList.Add(PurchaseStatus.Free);
                statusList.Add(PurchaseStatus.Gift);
            }

            if(FilterPreorder)
            {
                statusList.Add(PurchaseStatus.Preordered);
            }

            if(FilterWishlist)
            {
                statusList.Add(PurchaseStatus.Wishlist);
            }

            if (FilterAnnounced)
            {
                statusList.Add(PurchaseStatus.Announced);
            }

            if (FilterGone)
            {
                statusList.Add(PurchaseStatus.GiftedAway);
            }

            return statusList;
        }
    }
}
