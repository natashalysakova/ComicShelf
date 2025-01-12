using AutoMapper;
using Backend.Models;
using Backend.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using Services.Services.Enums;
using Services.ViewModels;
using System.Text;
using ComicType = Backend.Models.Enums.Type;

namespace Services.Services
{
    public class VolumeService : BasicService<Volume, VolumeViewModel, VolumeCreateModel, VolumeUpdateModel>
    {
        private readonly ComicShelfContext _context;
        private readonly SeriesService _seriesService;
        private readonly AuthorsService _authorService;
        private readonly PublishersService _publishersService;
        private readonly IssueService _issueService;
        private readonly ILocalizationService _localizationService;

        public VolumeService(ComicShelfContext context, SeriesService seriesService, AuthorsService authorService, PublishersService publishersService, IssueService issueService, ILocalizationService localizationService, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _seriesService = seriesService;
            _authorService = authorService;
            _publishersService = publishersService;
            _issueService = issueService;
            _localizationService = localizationService;
        }

        public override IEnumerable<VolumeViewModel> GetAll()
        {
            return _mapper.ProjectTo<VolumeViewModel>(GetAllEntities().Include(x => x.Series));
        }

        public VolumeViewModel Get(int id)
        {
            var volume = GetById(id);
            LoadReference(volume, x => x.Series);
            LoadCollection(volume, x => x.Authors);
            LoadCollection(volume, x => x.Issues);
            LoadReference(volume, x => x.History);
            volume.Issues = volume.Issues.OrderBy(x => x.Number).ToList();
            _seriesService.LoadReference(volume.Series, x => x.Publisher);
            _publishersService.LoadReference(volume.Series.Publisher, x => x.Country);

            return _mapper.Map<VolumeViewModel>(volume);
        }

        public override int Add(VolumeCreateModel model)
        {
            var series = _seriesService.GetByName(model.SeriesName);
            if (series == null)
            {
                var newSeries = new SeriesCreateModel() { Name = model.SeriesName, Type = ComicType.Manga, PublisherName = model.PublisherName, OriginalName = model.SeriesOriginalName };
                if (model.VolumeType == VolumeItemType.OneShot)
                {
                    newSeries.TotalVolumes = 1;
                    newSeries.Ongoing = false;

                    if (model.PurchaseStatus != PurchaseStatus.Announced || model.PurchaseStatus != PurchaseStatus.GiftedAway)
                    {
                        newSeries.Completed = true;
                    }
                }
                else
                {
                    newSeries.TotalVolumes = model.TotalVolumes;
                    if (model.SeriesStatus == "ongoing")
                    {
                        newSeries.Ongoing = true;
                    }
                }
                var id = _seriesService.Add(newSeries);
                series = _seriesService.GetById(id);
            }
            else
            {
                if (series.TotalVolumes < model.TotalVolumes)
                {
                    series.TotalVolumes = model.TotalVolumes;
                }

                if (series.Ongoing && model.SeriesStatus == "finished")
                {
                    series.Ongoing = false;
                }

                if (string.IsNullOrEmpty(series.OriginalName) && !string.IsNullOrEmpty(model.SeriesOriginalName))
                {
                    series.OriginalName = model.SeriesOriginalName;
                }

                SaveChanges();
            }

            var authorList = new List<int>();
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
                        authorList.Add(_authorService.Add(new AuthorCreateModel() { Name = trimmedItem, Roles = role }));
                    }
                    else
                    {
                        authorList.Add(author.Id);
                    }

                }
            }

            if (model.CoverFile is not null)
                model.CoverUrl = FileUtility.SaveOnServer(model.CoverFile, series.Name, model.Number);
            else if (model.CoverToDownload is not null)
                model.CoverUrl = FileUtility.DownloadFileFromWeb(model.CoverToDownload, series.Name, model.Number);
            else
                model.CoverUrl = "images\\static\\no-cover.png";

            var volume = _mapper.Map<Volume>(model);

            switch (model.PurchaseStatus)
            {
                case PurchaseStatus.Announced:
                    volume.ReleaseDate = model.ReleaseDate;
                    volume.PurchaseDate = default;
                    volume.PreorderDate = default;
                    break;
                case PurchaseStatus.Preordered:
                    volume.ReleaseDate = model.ReleaseDate;
                    volume.PreorderDate = model.PreorderDate;
                    volume.PurchaseDate = default;
                    break;
                case PurchaseStatus.Wishlist:
                    volume.ReleaseDate = default;
                    volume.PreorderDate = default;
                    volume.PurchaseDate = default;
                    break;
                default:
                    volume.ReleaseDate = default;
                    volume.PreorderDate = default;
                    volume.PurchaseDate = model.PurchaseDate;
                    break;
            }

            if (model.Status is Status.Dropped or Status.Completed)
            {
                volume.Rating = model.Rating;
            }



            _seriesService.LoadCollection(series, x => x.Volumes);

            if ((volume.PurchaseStatus is PurchaseStatus.Bought
                or PurchaseStatus.Pirated
                or PurchaseStatus.Gift
                or PurchaseStatus.Free) && series.Volumes.Count > 0 && series.Volumes.All(x => x.Status == Status.Completed) && volume.Status == Status.NotStarted)
            {
                volume.Status = Status.InQueue;
            }

            volume.Series = series;
            volume.Authors = _authorService.GetAll(authorList);

            if (model.VolumeType == VolumeItemType.OneShot)
            {
                volume.Number = 1;
                volume.OneShot = true;
            }

            if (model.VolumeType == VolumeItemType.Issue)
            {
                var issue = new Issue()
                {
                    Volume = volume,
                    Number = model.Number,
                    Name = model.Title
                };

                _context.Issues.Add(issue);
            }
            else
            {
                int maxChapter = 0;

                foreach (var item in series.Volumes)
                {
                    LoadCollection(item, x => x.Issues);

                    foreach (var item1 in item.Issues)
                    {
                        if (item1 is not Bonus && item1.Number > maxChapter)
                        {
                            maxChapter = item1.Number;
                        }
                    }
                }
                var chapterName = _localizationService["ChapterDefaultName"];

                for (int i = 0; i < model.NumberOfIssues; i++)
                {
                    var issue = new Issue()
                    {
                        Volume = volume,
                        Number = maxChapter + 1,
                        Name = $"{chapterName} {maxChapter + 1}"
                    };

                    volume.Issues.Add(issue);
                    maxChapter++;
                }
            }

            volume.SingleIssue = volume.Issues.Count == 1;

            var bonusChapterName = _localizationService["BonusChapterDefaultName"];

            for (int i = 0; i < model.NumberOfBonusIssues; i++)
            {
                var issue = new Bonus()
                {
                    Volume = volume,
                    Number = i + 1,
                    Name = $"{bonusChapterName} {i + 1}"
                };

                volume.Issues.Add(issue);
            }


            if (!series.Ongoing && series.Volumes.Count(x => x.Number > 0) == series.TotalVolumes)
            {
                series.Completed = true;
            }


            volume.CreationDate = DateTime.Now;
            volume.ModificationDate = DateTime.Now;

            return Add(volume);
        }

        private IEnumerable<Issue> GenerateEmptyIssues(int issues, ComicType type)
        {
            for (int i = 1; i <= issues; i++)
            {
                yield return new Issue() { Number = i, Name = type == ComicType.Comics ? "Issue " + i : "Chapter " + i };
            }
        }

        public bool Exists(string seriesName, int volumeNumber)
        {
            return dbSet.Any(x => x.Series.Name == seriesName && x.Number == volumeNumber);
        }

        public override void Update(VolumeUpdateModel volumeToUpdate)
        {
            var item = GetById(volumeToUpdate.Id);
            if (item == null)
            {
                return;
            }

            item.PurchaseStatus = volumeToUpdate.PurchaseStatus;
            item.Status = volumeToUpdate.Status;

            if (volumeToUpdate.Status is Status.Completed or Status.Dropped)
            {
                item.Rating = volumeToUpdate.Rating;
            }

            if (volumeToUpdate.PurchaseStatus is not PurchaseStatus.Announced or PurchaseStatus.Preordered or PurchaseStatus.Wishlist && volumeToUpdate.PurchaseDate != default)
            {
                item.PurchaseDate = volumeToUpdate.PurchaseDate;
            }

            if (volumeToUpdate.PurchaseStatus is PurchaseStatus.Announced or PurchaseStatus.Preordered && volumeToUpdate.ReleaseDate != default)
            {
                item.ReleaseDate = volumeToUpdate.ReleaseDate;
            }

            if (volumeToUpdate.PurchaseStatus is PurchaseStatus.Preordered && volumeToUpdate.PreorderDate != default)
            {
                item.PreorderDate = volumeToUpdate.PreorderDate;
            }

            if (volumeToUpdate.CoverFile is not null)
            {
                LoadReference(item, x => x.Series);
                item.CoverUrl = FileUtility.SaveOnServer(volumeToUpdate.CoverFile, item.Series.Name, item.Number);
            }

            var series = _seriesService.GetById(item.SeriesId);
            if (volumeToUpdate.PurchaseStatus is PurchaseStatus.Bought
                or PurchaseStatus.Pirated
                or PurchaseStatus.Gift
                or PurchaseStatus.Free)
            {
                _seriesService.LoadCollection(series, x => x.Volumes);

                var volumesExceptCurrent = series.Volumes.Except(new[] { item });

                if (volumesExceptCurrent.Count() > 0 && volumesExceptCurrent.All(x => x.Status == Status.Completed) && item.Status == Status.NotStarted)
                {
                    item.Status = Status.InQueue;
                }
            }



            if (item.SeriesId == 0)
                item.SeriesId = _seriesService.GetByName(item.Series.Name).Id;

            if (volumeToUpdate.Issues != null)
            {
                item.Issues.AddRange(_mapper.Map<Issue[]>(volumeToUpdate.Issues));
            }
            if (volumeToUpdate.BonusIssues != null)
            {
                item.Issues.AddRange(_mapper.Map<Issue[]>(volumeToUpdate.BonusIssues));
            }

            item.OneShot = series.TotalVolumes == 1;

            if (
                (volumeToUpdate.Issues != null && volumeToUpdate.Issues?.Length == 1) ||
                (volumeToUpdate.Issues is null && volumeToUpdate.BonusIssues is not null && volumeToUpdate.BonusIssues.Length > 0))
            {
                item.SingleIssue = true;
            }

            if (volumeToUpdate.Issues is null && volumeToUpdate.BonusIssues is not null)
            {
                item.Number = 0;
            }


            if (item.CreationDate == default)
                item.CreationDate = DateTime.Now;

            item.ModificationDate = DateTime.Now;

            base.Update(item);

            if (item.PurchaseStatus != PurchaseStatus.Announced || item.PurchaseStatus != PurchaseStatus.GiftedAway)
            {
                if (!series.Ongoing && series.Volumes.Count(x => x.Number > 0) == series.TotalVolumes)
                {
                    series.Completed = true;
                    _seriesService.Update(series);
                }
            }


            if (item.Status == Status.Completed)
            {
                var nextChapter = dbSet.Where(x => x.SeriesId == item.SeriesId && x.Number > item.Number && x.Status == Status.NotStarted && x.PurchaseStatus != PurchaseStatus.Announced && x.PurchaseStatus != PurchaseStatus.Preordered).OrderBy(x => x.Number).FirstOrDefault();
                if (nextChapter != null)
                {
                    nextChapter.Status = Status.InQueue;
                    Update(nextChapter);
                }
            }


        }

        public IEnumerable<VolumeViewModel> Filter(BookshelfParams param)
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



            var filterd = GetAllEntities()
                .Include(x => x.Series)
                    .ThenInclude(x => x.Publisher)
                    .ThenInclude(x => x.Country)
                .Include(x => x.Authors)
                .Where(x => statusList.Contains(x.PurchaseStatus));


            switch (param.digitality)
            {
                case DigitalityEnum.Physical:
                    filterd = filterd.Where(x => x.Digitality == VolumeType.Physical);
                    break;
                case DigitalityEnum.Digital:
                    filterd = filterd.Where(x => x.Digitality == VolumeType.Digital);
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
                    x.Series.Publisher.Name.ToLower().Contains(param.search)
                );
            }

            switch (param.reading)
            {
                case ReadingEnum.NotStarted:
                    filterd = filterd.Where(x => x.Status == Status.NotStarted);
                    break;
                case ReadingEnum.NewSeries:
                    filterd = filterd.Where(x => x.Status == Status.NotStarted && x.Number < 2);
                    break;
                case ReadingEnum.InQueue:
                    filterd = filterd.Where(x => x.Status == Status.InQueue);
                    break;
                case ReadingEnum.Reading:
                    filterd = filterd.Where(x => x.Status == Status.Reading);
                    break;
                case ReadingEnum.Completed:
                    filterd = filterd.Where(x => x.Status == Status.Completed);
                    break;
                case ReadingEnum.Dropped:
                    filterd = filterd.Where(x => x.Status == Status.Dropped);
                    break;
            }

            ///<option value="0" selected>За датою додавання</option>
            ///<option value = "1" > За назвою </ option >
            ///<option value = "2" > За датою покупки</ option >
            ///<option value = "3" > Three </ option >
            ///

            switch (param.volumeType)
            {
                case VolumeItemType.Issue:
                    filterd = filterd.Where(x => x.SingleIssue);
                    break;
                case VolumeItemType.OneShot:
                    filterd = filterd.Where(x => x.OneShot);
                    break;
                case VolumeItemType.Volume:
                    filterd = filterd.Where(x => !x.OneShot && !x.SingleIssue);
                    break;
            }

            switch (param.sort)
            {
                case SortEnum.BySeriesTitle:
                    filterd = filterd.OrderByDescending(x => x.Series.Name).ThenByDescending(x => x.Number);
                    break;
                case SortEnum.ByPurchaseDate:
                    filterd = filterd.OrderBy(item => item.PurchaseDate)
                                    .ThenBy(item => item.PreorderDate)
                                    .ThenBy(item => item.ReleaseDate)
                                    .ThenBy(x => x.Series.Name)
                                    .ThenBy(x => x.Number);
                    break;
                default:
                    filterd = filterd.OrderBy(x => x.CreationDate);
                    break;
            }


            if (param.direction == DirectionEnum.up)
            {
                filterd = filterd.Reverse();
            }

            return _mapper.ProjectTo<VolumeViewModel>(filterd).ToList();
        }

        public override string SetNotificationMessage()
        {
            var builder = new StringBuilder();
            if (GetAllEntities().ToList().Any(x => x.Expired()))
            {
                builder.Append("There are expired preorders");
            }

            return builder.ToString();
        }

        public void AddIssues(int volumeId, int issueNumber, int bonusIssueNumber)
        {
            var volume = GetById(volumeId);
            LoadReference(volume, x => x.Series);
            _seriesService.LoadCollection(volume.Series, x => x.Volumes);

            int maxChapter = 0;
            int maxBonusChapter = 0;

            foreach (var item in volume.Series.Volumes)
            {
                LoadCollection(item, x => x.Issues);

                foreach (var item1 in item.Issues)
                {
                    if (item1 is not Bonus && item1.Number > maxChapter)
                    {
                        maxChapter = item1.Number;
                    }
                }
            }

            foreach (var item in volume.Series.Volumes)
            {
                foreach (var item1 in item.Issues)
                {
                    if (item1 is Bonus && item1.Number > maxBonusChapter)
                    {
                        maxBonusChapter = item1.Number;
                    }
                }
            }

            var chapterName = _localizationService["ChapterDefaultName"];
            var bonusChapterName = _localizationService["BonusChapterDefaultName"];

            for (int i = 0; i < issueNumber; i++)
            {
                var issue = new Issue()
                {
                    Volume = volume,
                    Number = maxChapter + 1,
                    Name = $"{chapterName} {maxChapter + 1}"
                };

                _context.Issues.Add(issue);
                maxChapter++;
            }
            for (int i = 0; i < bonusIssueNumber; i++)
            {
                var issue = new Bonus()
                {
                    Volume = volume,
                    Number = maxBonusChapter + 1,
                    Name = $"{bonusChapterName} {maxBonusChapter + 1}"
                };

                _context.Issues.Add(issue);
                maxBonusChapter++;
            }

            volume.SingleIssue = issueNumber == 1;

            _context.SaveChanges();
        }

        public void DeleteIssue(int id)
        {
            var issue = _issueService.GetById(id);
            _issueService.Remove(id);

            var volume = GetById(issue.VolumeId);
            LoadCollection(volume, x => x.Issues);

            volume.SingleIssue = volume.Issues.Count(x => x.GetType() == typeof(Issue)) == 1;
            _context.SaveChanges();

        }
    }
}
