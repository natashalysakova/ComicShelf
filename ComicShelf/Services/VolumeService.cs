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
using System.Text;
using ComicShelf.ViewModels;

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

        public void Add(VolumeCreateModel model)
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
                var splitted = item.Split(',', StringSplitOptions.RemoveEmptyEntries);

                foreach (var splittedItem in splitted)
                {
                    var trimmedItem = splittedItem.Trim();
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
            }

            var issues = GenerateEmptyIssues(model.Issues, series.Type).ToList();

            string? urlPath;
            if (model.CoverFile != null)
                urlPath = FileUtility.SaveOnServer(model.CoverFile, series.Name, model.Number);
            else
                urlPath = "images\\static\\no-cover.png";

            var volume = new Volume()
            {
                Title = model.Title,
                Issues = issues,
                Series = series,
                Authors = authorList,
                Number = model.Number,
                PurchaseDate = model.PurchaseDate,
                PurchaseStatus = model.PurchaseStatus,
                Rating = model.Rating,
                Status = model.Status,
                CoverUrl = urlPath,
                //Cover = cover,
                CreationDate = DateTime.Now,
                ModificationDate = DateTime.Now,
                ReleaseDate = model.ReleaseDate
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
            return base.GetAll().Include(x => x.Series).Include(x => x.Authors);
        }

        public override void Update(Volume item)
        {
            if (item.CreationDate == default)
                item.CreationDate = DateTime.Now;

            item.ModificationDate = DateTime.Now;

            if (item.SeriesId == 0)
                item.SeriesId = _seriesService.GetByName(item.Series.Name).Id;

            base.Update(item);
        }

        public IList<Volume> Filter(BookshelfParams param)
        {
            IList<PurchaseStatus> statusList = new List<PurchaseStatus>();

            switch (param.filter)
            {
                case PurchaseFilterEnum.Available:
                    statusList.Add(PurchaseStatus.Bought);
                    statusList.Add(PurchaseStatus.Free);
                    statusList.Add(PurchaseStatus.Gift);
                    break;
                case PurchaseFilterEnum.Preorders:
                    statusList.Add(PurchaseStatus.Preordered);
                    break;
                case PurchaseFilterEnum.Wishlist:
                    statusList.Add(PurchaseStatus.Wishlist);
                    break;
                case PurchaseFilterEnum.Announced:
                    statusList.Add(PurchaseStatus.Announced);
                    break;
                case PurchaseFilterEnum.Gone:
                    statusList.Add(PurchaseStatus.GiftedAway);
                    break;
                default:
                    statusList.AddRange(Enum.GetValues(typeof(PurchaseStatus)).Cast<PurchaseStatus>());
                    break;
            }



            var filterd = GetAll().Where(x => statusList.Contains(x.PurchaseStatus));


            switch (param.digitality)
            {
                case DigitalityEnum.Physical:
                    filterd = filterd.Where(x => x.Digitality == VolumeType.Physical);
                    break;
                case DigitalityEnum.Digital:
                    filterd = filterd.Where(x=>x.Digitality == VolumeType.Digital);
                    break;
                case DigitalityEnum.All:
                    break;
                default:
                    break;
            }



            if (!string.IsNullOrEmpty(param.search))
            {
                param.search = param.search.ToLower();

                filterd = filterd.Where(x =>
                    x.Title.ToLower().Contains(param.search) ||
                    x.Series.Name.ToLower().Contains(param.search) ||
                    x.Series.OriginalName.ToLower().Contains(param.search) ||
                    x.Authors.Any(y => y.Name.ToLower().Contains(param.search)) ||
                    x.Series.Publishers.Any(y => y.Name.ToLower().Contains(param.search))
                );
            }

            ///<option value="0" selected>За датою додавання</option>
            ///<option value = "1" > За назвою </ option >
            ///<option value = "2" > За датою покупки</ option >
            ///<option value = "3" > Three </ option >

            switch (param.sort)
            {
                case SortEnum.BySeriesTitle:
                    filterd = filterd.OrderBy(x => x.Series.Name).ThenBy(x => x.Number);
                    break;
                case SortEnum.ByPurchaseDate:
                    filterd = filterd.OrderBy(x => x.PurchaseDate).ThenBy(x => x.Series.Name).ThenBy(x => x.Number);
                    break;
                default:
                    filterd = filterd.OrderBy(x => x.CreationDate);
                    break;
            }


            if (param.direction == DirectionEnum.up)
            {
                filterd = filterd.Reverse();
            }

            return filterd.ToList();
        }

        internal void UpdatePurchaseStatus(VolumeUpdateModel volumeToUpdate)
        {
            var item = Get(volumeToUpdate.Id);
            if (item != null)
            {
                item.PurchaseStatus = volumeToUpdate.PurchaseStatus;
                item.Status = volumeToUpdate.Status;

                item.Rating = volumeToUpdate.Rating;

                if (volumeToUpdate.PurchaseDate != default)
                {
                    item.PurchaseDate = volumeToUpdate.PurchaseDate;
                }

                if (volumeToUpdate.ReleaseDate != default)
                {
                    item.ReleaseDate = volumeToUpdate.ReleaseDate;
                }

                if (volumeToUpdate.CoverFile != null)
                {
                    LoadReference(item, x => x.Series);
                    item.CoverUrl = FileUtility.SaveOnServer(volumeToUpdate.CoverFile, item.Series.Name, item.Number);
                }

                Update(item);
            }
        }

        public override string SetNotificationMessage()
        {
            var builder = new StringBuilder();
            if (GetAll().AsEnumerable().Any(x => x.Expired()))
            {
                builder.Append("There are volumes that need your attention");
            }

            return builder.ToString();
        }
    }
}
