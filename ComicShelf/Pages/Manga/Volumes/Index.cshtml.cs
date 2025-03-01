﻿using Backend.Models.Enums;
using ComicShelf.Localization;
using ComicShelf.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Packaging;
using Services.Services;
using Services.Services.Enums;
using Services.ViewModels;
using System.Net;
using ComicShelf.Utilities;
using ComicShelf.PublisherParsers;

namespace ComicShelf.Pages.Volumes
{
    public class IndexModel : PageModel
    {
        private readonly EnumUtilities _enumUtilities;
        private readonly IEnumerable<IPublisherParser> _parsers;
        private readonly FilterService _filterService;
        private readonly VolumeService _volumeService;
        private readonly SeriesService _seriesService;

        public IndexModel(VolumeService volumeService, SeriesService seriesService, AuthorsService authorsService, FilterService filterService, EnumUtilities enumUtilities, IEnumerable<IPublisherParser> parsers)
        {
            _volumeService = volumeService;
            _seriesService = seriesService;
            _filterService = filterService;
            _enumUtilities = enumUtilities;
            _parsers = parsers;
            Statuses.AddRange(_enumUtilities.GetSelectItemList<Status>());
            PurchaseStatuses.AddRange(_enumUtilities.GetSelectItemList<PurchaseStatus>());
            Ratings.AddRange(_enumUtilities.GetRatings());
            Digitalities.AddRange(_enumUtilities.GetSelectItemList<VolumeType>());
            VolumeTypes.AddRange(_enumUtilities.GetSelectItemList<VolumeItemType>());

            Authors.AddRange(authorsService.GetAll().Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }));
            Series.AddRange(seriesService.GetAll().Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }));
            Filters = filterService.GetAllForView();
        }



        public IEnumerable<VolumeViewModel> AnnouncedAndPreordered
        {
            get
            {
                return Volumes.Where(x => x.PurchaseStatus == PurchaseStatus.Announced || x.PurchaseStatus == Backend.Models.Enums.PurchaseStatus.Preordered).ToList();
            }
        }

        public List<SelectListItem> Authors { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> Digitalities { get; set; } = new List<SelectListItem>();

        public IEnumerable<IGrouping<string, FilterViewModel>> Filters { get; set; }

        [BindProperty]
        public VolumeCreateModel NewVolume { get; set; } = default!;

        //[BindProperty]
        //public string ImportUrl { get; set; } = default!;

        public IEnumerable<VolumeViewModel> Purchased
        {
            get
            {
                return Volumes.Except(AnnouncedAndPreordered).Except(WishList).ToList();
            }
        }

        public List<SelectListItem> PurchaseStatuses { get; set; } = new List<SelectListItem>();
        public List<int> Ratings { get; set; } = new List<int>();
        public List<SelectListItem> Series { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Statuses { get; set; } = new List<SelectListItem>();
        public IEnumerable<VolumeViewModel> Volumes { get; set; } = default!;
        public List<SelectListItem> VolumeTypes { get; set; } = new List<SelectListItem>();
        public IEnumerable<VolumeViewModel> WishList
        {
            get
            {
                return Volumes.Where(x => x.PurchaseStatus == Backend.Models.Enums.PurchaseStatus.Wishlist).ToList();
            }
        }
        public void OnGetAsync()
        {
            //Volumes = await volumeService.GetAll().Include(x => x.Series).OrderBy(x => x.Series.Name).ThenBy(x => x.Number).ToListAsync();
            var filters = FromCookies();

            Volumes = _volumeService.Filter(filters);
            ViewData["DigitalityFilters"] = _enumUtilities.GetListOfValues<DigitalityEnum>();
            ViewData["PurchaseFilters"] = _enumUtilities.GetListOfValues<PurchaseFilterEnum>();
            ViewData["Sort"] = _enumUtilities.GetListOfValues<SortEnum>();
            ViewData["ReadingFilters"] = _enumUtilities.GetListOfValues<ReadingEnum>();
            ViewData["VolumeTypesFilters"] = _enumUtilities.GetListOfValues<VolumeItemType>();
            SetCookies(filters);

            NewVolume = new VolumeCreateModel();
        }

        public IActionResult OnGetDeleteChapter(int id)
        {
            _volumeService.DeleteIssue(id);
            return StatusCode(200);
        }

        public PartialViewResult OnGetFiltered(BookshelfParams filters)
        {
            Volumes = _volumeService.Filter(filters);
            SetCookies(filters);
            return Partial("_ShelfPartial", Volumes);
        }

        public PartialViewResult OnGetVolumeAsync(int id)
        {
            if (id is <= 0)
            {
                throw new ArgumentException("Invalid Id");
            }

            var volume = _volumeService.Get(id);
            var resultVD = new ViewDataDictionary<VolumeViewModel>(ViewData, volume);
            resultVD["PurchaseStatuses"] = _enumUtilities.GetPurchaseStatusesSelectItemList(volume.PurchaseStatus);
            resultVD["ReadingStatuses"] = _enumUtilities.GetSelectItemList<Status>();
            resultVD["Ratings"] = _enumUtilities.GetRatings();
            resultVD["Digitality"] = _enumUtilities.GetSelectItemList<VolumeType>();
            return new PartialViewResult()
            {
                ViewName = "_VolumePartial",
                ViewData = resultVD
            };
        }

        public IActionResult OnPostAddAsync()
        {
            if (!ModelState.IsValid || NewVolume == null)
            {
                return StatusCode(400, "Fill mandatory fields " + string.Join(',', ModelState.Where(x=>x.Value.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid).Select(x=>x.Key)));
            }

            if (_volumeService.Exists(NewVolume.SeriesName, NewVolume.Number))
            {
                return StatusCode(409, $"{NewVolume.SeriesName} Volume #{NewVolume.Number} already exists");
            }

            _volumeService.Add(NewVolume);

            Volumes = _volumeService.Filter(FromCookies());
            return Partial("_ShelfPartial", Volumes);
        }

        public IActionResult OnPostAddChapters(int volumeId, int issueNumber, int bonusIssueNumber)
        {
            _volumeService.AddIssues(volumeId, issueNumber, bonusIssueNumber);

            var volume = _volumeService.Get(volumeId);
            return Partial("_ChaptersView", volume);
        }

        public IActionResult OnPostChangeStatus(VolumeUpdateModel volumeToUpdate)
        {
            _volumeService.Update(volumeToUpdate);
            return this.StatusCode(HttpStatusCode.OK);
        }

        public IActionResult OnPostSaveFilter(string filterName)
        {
            var filters = FromCookies();

            _filterService.Add(new FilterCreateModel() { Name = filterName, Json = JsonConvert.SerializeObject(filters) });

            return new JsonResult(_filterService.GetAllForView());
        }

        public BookshelfParams FromCookies()
        {
            var cookie = Request.Cookies["filters"];
            if (cookie != null)
            {
                var fromCookie = JsonConvert.DeserializeObject<BookshelfParams>(cookie);
                if (fromCookie != null)
                {
                    return fromCookie;
                }
            }

            return new BookshelfParams();
        }

        private void SetCookies(BookshelfParams filters)
        {
            Response.Cookies.Append("filters", JsonConvert.SerializeObject(filters));
        }

        public async Task<IActionResult> OnPostParseUrl(string importUrl)
        {
            if (string.IsNullOrEmpty(importUrl))
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            var factory = new PublisherParsersFactory();
            var parser = factory.CreateParser(importUrl);

            if (parser == null)
            {
                return this.StatusCode(HttpStatusCode.NotFound);
            }

            var data = await parser.Parse();

            return new JsonResult(data);

        }

        public IActionResult OnGetSeriesAuthor(string series)
        {
            var author = _seriesService.GetSeriesAuthor(series);
            if (author is null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }

            return Content(author);
        }
    }
}