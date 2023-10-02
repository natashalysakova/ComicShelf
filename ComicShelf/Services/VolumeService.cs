using ComicShelf.Models;
using ComicShelf.Models.Enums;
using ComicShelf.Pages.Volumes;
using Microsoft.Extensions.Hosting.Internal;
using System.Security.Policy;
using UnidecodeSharpFork;
using static System.Net.Mime.MediaTypeNames;
using System.Web;
using ComicType = ComicShelf.Models.Enums.Type;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using System.Linq.Expressions;

namespace ComicShelf.Services
{
    public class VolumeService : BasicService<Volume>
    {
        private readonly SeriesService _seriesService;
        private readonly AuthorsService _authorService;

        public VolumeService(ComicShelfContext context, SeriesService seriesService, AuthorsService authorService) : base(context)
        {
            _seriesService = seriesService;
            _authorService = authorService;
        }

        public void Add(VolumeModel model)
        {
            var series = _seriesService.GetByName(model.Series);
            if (series == null)
            {
                series = new Series() { Name = model.Series, Type = ComicType.Comics };
                _seriesService.Add(series);
            }

            var authorList = new List<Author>();
            foreach (var item in model.Authors)
            {
                var trimmedItem = item.Trim();
                if (string.IsNullOrEmpty(trimmedItem))
                {
                    continue;
                }

                var author = _authorService.GetByName(trimmedItem);
                if (author == null)
                {
                    Roles role;

                    if (series.Type == ComicType.Manga)
                    {
                        role = Roles.Mangaka;
                    }
                    else
                    {
                        role = Roles.None;
                    }

                    author = new Author() { Name = trimmedItem, Roles = role };
                    _authorService.Add(author);
                }

                authorList.Add(author);
            }

            var issues = GenerateEmptyIssues(model.Issues, series.Type).ToList();

            var urlPath = string.Empty;
            //var cover = new VolumeCover();
            //if (model.CoverFile != null && model.CoverFile.Length > 0)
            //{
            urlPath = FileUtility.SaveOnServer(model.CoverFile, series.Name, model.Number, out string extention);
            //    cover.Cover = coverBytes;
            //    cover.Extention = extention;
            //}

            var volume = new Volume()
            {
                Title = model.Title,
                Issues = issues,
                Series = series,
                Authors = authorList,
                Number = model.Number,
                PurchaseDate = model.PurchaseDate,
                PurchaseStatus = model.PurchaseStatus,
                Raiting = model.Raiting,
                Status = model.Status,
                CoverUrl = urlPath,
                //Cover = cover,
                CreationDate = DateTime.Now,
            };

            this.Add(volume);
        }

        private IEnumerable<Issue> GenerateEmptyIssues(int issues, ComicType type)
        {
            for (int i = 1; i <= issues; i++)
            {
                yield return new Issue() { Number = i, Name = type == ComicType.Comics ? "Issue " + i : "Chapter " + i };
            }
        }

        internal string GetSeriesName(Volume volume)
        {
            return _seriesService.GetAll().Single(x => x.Volumes.Contains(volume)).Name;
        }

        public bool Exists(string seriesName, int volumeNumber)
        {
            return dbSet.Any(x => x.Series.Name == seriesName && x.Number == volumeNumber);
        }

        public override IQueryable<Volume> GetAll()
        {
            return base.GetAll().Include(x => x.Series).Include(x=>x.Authors);
        }

        public override void Update(Volume item)
        {
            //if (item.CoverUrl.StartsWith("http"))
            //{
            //    var seriesName = GetSeriesName(item);
            //    item.CoverUrl = FileUtility.SaveOnServer(model.CoverFile, seriesName, item.Number, out byte[] coverBytes, out string extention);

            //    var cover = _coverService.GetCoverForVolume(item);
            //    if(cover == null)
            //    {
            //        var original = Get(item.Id);
            //        cover = new VolumeCover() { Cover = coverBytes, Extention = extention };
            //        original.Cover = cover;
            //        _coverService.Add(cover);
            //    }
            //    else
            //    {
            //        cover.Cover = coverBytes;
            //        cover.Extention = extention;
            //        _coverService.Update(cover);
            //    }
            //}

            base.Update(item);
        }

        public IList<Volume> Filter(BookshelfParams param)
        {
            IList<PurchaseStatus> statusList = new List<PurchaseStatus>();

            switch (param.filter)
            {
                case "FilterAvailable":
                    statusList.Add(PurchaseStatus.Bought);
                    statusList.Add(PurchaseStatus.Free);
                    statusList.Add(PurchaseStatus.Gift);
                    break;
                case "FilterPreorder":
                    statusList.Add(PurchaseStatus.Preordered);
                    break;
                case "FilterWishlist":
                    statusList.Add(PurchaseStatus.Wishlist);
                    break;
                case "FilterAnnounced":
                    statusList.Add(PurchaseStatus.Announced);
                    break;
                case "FilterGone":
                    statusList.Add(PurchaseStatus.GiftedAway);
                    break;
                default:
                    statusList.AddRange(Enum.GetValues(typeof(PurchaseStatus)).Cast<PurchaseStatus>());
                    break;
            }

            var filterd = GetAll().Where(x => statusList.Contains(x.PurchaseStatus));

            if (!string.IsNullOrEmpty(param.search))
            {
                param.search = param.search.ToLower();

                filterd = filterd.Where(x =>
                    x.Title.ToLower().Contains(param.search) ||
                    x.Series.Name.ToLower().Contains(param.search) || 
                    x.Series.OriginalName.ToLower().Contains(param.search) || 
                    x.Authors.Any(y=>y.Name.ToLower().Contains(param.search)) ||
                    x.Series.Publishers.Any(y=>y.Name.ToLower().Contains(param.search)) 
                );
            }

            //<option value="0" selected>За датою додавання</option>
            //<option value = "1" > За назвою </ option >
            //<option value = "2" > За датою покупки</ option >
            //<option value = "3" > Three </ option >

            switch (param.sort)
            {
                case 1:
                    filterd = filterd.OrderBy(x => x.Series.Name).ThenBy(x=>x.Number);
                    break;
                case 2:
                    filterd = filterd.OrderBy(x => x.PurchaseDate).ThenBy(x=>x.Series.Name).ThenBy(x=>x.Number);
                    break;
                default:
                    filterd = filterd.OrderBy(x => x.CreationDate);
                    break;
            }


            if (param.direction == "up")
            {
                filterd = filterd.Reverse();
            }

            return filterd.ToList();
        }

        internal void UpdatePurchaseStatus(int id, string purchaseStatus)
        {
            var item = Get(id);
            if (item != null)
            {
                item.PurchaseStatus = (PurchaseStatus)Enum.Parse(typeof(PurchaseStatus), purchaseStatus);
                Update(item);
            }
        }
    }
}
