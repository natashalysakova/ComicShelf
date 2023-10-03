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
            Volumes = volumeService.Filter(new BookshelfParams());
            ViewData["Title"] = _localizer.GetString("Main page");
        }

        public PartialViewResult OnGetVolumeAsync(int id)
        {
            var volume = volumeService.Get(id);
            volumeService.LoadReference(volume, x => x.Series);
            volumeService.LoadCollection(volume, x => x.Authors);
            return Partial("_VolumePartial", volume);
        }

        public async Task<IActionResult> OnPostChangeStatus(int id, string purchaseStatus, DateTime purchaseDate, DateTime releaseDate)
        {
            volumeService.UpdatePurchaseStatus(id, purchaseStatus, purchaseDate, releaseDate);
            var item = volumeService.Get(id);
            volumeService.LoadReference(item, x => x.Series);
            var partial = Partial("_BookPartial", item);
            return partial;
        }

        public async Task<IActionResult> OnPostAddAsync()
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

            Volumes = volumeService.Filter(new BookshelfParams());
            return Partial("_ShelfPartial", Volumes);
        }

        public PartialViewResult OnGetFiltered(BookshelfParams filters)
        {
            Volumes = volumeService.Filter(filters);
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

    public class BookshelfParams
    {
        //public bool FilterAvailable { get; set; }
        //public bool FilterPreorder { get; set; }
        //public bool FilterWishlist { get; set; }
        //public bool FilterAnnounced { get; set; }
        //public bool FilterGone { get; set; }

        //Dictionary<string, bool> filters = new Dictionary<string, bool>();
        public string? filter { get; set; }
        public int? sort { get; set; }
        public string? direction { get; set; }

        public string? search { get; set; }

        public BookshelfParams()
        {
            direction = "up";
            sort = 2;

        }
    }
}
