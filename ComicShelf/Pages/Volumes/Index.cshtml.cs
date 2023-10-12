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
using ComicShelf.ViewModels;
using Newtonsoft.Json;

namespace ComicShelf.Pages.Volumes
{
    public class IndexModel : PageModel
    {
        private readonly VolumeService _volumeService;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly EnumUtilities _enumUtilities;
        public IndexModel(VolumeService volumeService, SeriesService seriesService, AuthorsService authorsService, IStringLocalizer<SharedResource> localizer, EnumUtilities enumUtilities)
        {
            _volumeService = volumeService;
            _localizer = localizer;
            _enumUtilities = enumUtilities;

            Statuses.AddRange(_enumUtilities.GetStatusSelectItemList());
            PurchaseStatuses.AddRange(_enumUtilities.GetPurchaseStatusSelectItemList());
            Ratings.AddRange(_enumUtilities.GetRatings());
            Digitalities.AddRange(_enumUtilities.GetDigitalitySelectItemList());

            Authors.AddRange(authorsService.GetAll().Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }));
            Series.AddRange(seriesService.GetAll().Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }));

        }

        [BindProperty]
        public VolumeCreateModel NewVolume { get; set; } = default!;

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
            var filters = FromCookies();

            Volumes = _volumeService.Filter(filters);
            ViewData["DigitalityFilters"] = _enumUtilities.GetListOfValues<DigitalityEnum>();
            ViewData["PurchaseFilters"] = _enumUtilities.GetListOfValues<PurchaseFilterEnum>();
            ViewData["Sort"] = _enumUtilities.GetListOfValues<SortEnum>();
            ViewData["ReadingFilters"] = _enumUtilities.GetListOfValues<ReadingEnum>();
            SetCookies(filters);
        }


        internal BookshelfParams FromCookies()
        {
            //if (Enum.TryParse(Request.Cookies[nameof(filters.sort)], out SortEnum sort))
            //{
            //    filters.sort = sort;
            //}

            //if (Enum.TryParse(Request.Cookies[nameof(filters.digitality)], out DigitalityEnum digitality))
            //{
            //    filters.digitality = digitality;
            //}
            //if (Enum.TryParse(Request.Cookies[nameof(filters.filter)], out PurchaseFilterEnum purchaseFilter))
            //{
            //    filters.filter = purchaseFilter;

            //}
            //if (Enum.TryParse(Request.Cookies[nameof(filters.direction)], out DirectionEnum direction))
            //{
            //    filters.direction = direction;
            //}
            //if (Enum.TryParse(Request.Cookies[nameof(filters.reading)], out ReadingEnum reading))
            //{
            //    filters.reading = reading;
            //}
            var cookie = Request.Cookies["filters"];
            if(cookie != null)
            {
                var fromCookie = JsonConvert.DeserializeObject<BookshelfParams>(cookie);
                if(fromCookie != null)
                {
                    return fromCookie;
                }
            }

            return new BookshelfParams();
        }

        private void SetCookies(BookshelfParams filters)
        {
            //Response.Cookies.Append(nameof(filters.sort), filters.sort.ToString());
            //Response.Cookies.Append(nameof(filters.digitality), filters.digitality.ToString());
            //Response.Cookies.Append(nameof(filters.filter), filters.filter.ToString());
            //Response.Cookies.Append(nameof(filters.direction), filters.direction.ToString());
            //Response.Cookies.Append(nameof(filters.reading), filters.reading.ToString());

            Response.Cookies.Append("filters", JsonConvert.SerializeObject(filters));
        }

        public PartialViewResult OnGetVolumeAsync(int id)
        {
            var volume = _volumeService.Get(id);
            _volumeService.LoadReference(volume, x => x.Series);
            _volumeService.LoadCollection(volume, x => x.Authors);

            var resultVD = new ViewDataDictionary<Volume>(ViewData, volume);
            resultVD["PurchaseStatuses"] = _enumUtilities.GetPurchaseStatusesSelectItemList(volume.PurchaseStatus);
            resultVD["ReadingStatuses"] = _enumUtilities.GetStatusSelectItemList();
            resultVD["Ratings"] = _enumUtilities.GetRatings();
            resultVD["Digitality"] = _enumUtilities.GetDigitalitySelectItemList();
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
            if (item != null)
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
            SetCookies(filters);
            return Partial("_ShelfPartial", Volumes);
        }

    }
}
