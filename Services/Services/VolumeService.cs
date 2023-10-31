using ComicType = Backend.Models.Enums.Type;
using Microsoft.EntityFrameworkCore;

using System.Text;
using Backend.Models;
using Backend.Models.Enums;
using Services.Services.Enums;
using NuGet.Packaging;
using Services.ViewModels;
using AutoMapper;

namespace Services.Services
{
    public class VolumeService : BasicService<Volume, VolumeViewModel, VolumeCreateModel, VolumeUpdateModel>
    {
        private readonly SeriesService _seriesService;
        private readonly AuthorsService _authorService;
        private readonly PublishersService _publishersService;

        public VolumeService(ComicShelfContext context, SeriesService seriesService, AuthorsService authorService, PublishersService publishersService, IMapper mapper) : base(context, mapper)
        {
            _seriesService = seriesService;
            _authorService = authorService;
            _publishersService = publishersService;
        }

        public override IEnumerable<VolumeViewModel> GetAll()
        {
            return _mapper.ProjectTo<VolumeViewModel>(GetAllEntities().Include(x=>x.Series));
        }

        public VolumeViewModel Get(int id)
        {
            var volume = GetById(id);
            LoadReference(volume, x => x.Series);
            LoadCollection(volume, x => x.Authors);
            _seriesService.LoadReference(volume.Series, x => x.Publisher);
            _publishersService.LoadReference(volume.Series.Publisher, x => x.Country);

            return _mapper.Map<VolumeViewModel>(volume);
        }

        public override int Add(VolumeCreateModel model)
        {
            var series = _seriesService.GetByName(model.SeriesName);
            if (series == null)
            {
                var newSeries = new SeriesCreateModel() { Name = model.SeriesName, Type = ComicType.Comics};
                if (model.SingleVolume)
                {
                    newSeries.TotalVolumes = 1;
                    newSeries.Ongoing = false;

                    if (model.PurchaseStatus != PurchaseStatus.Announced || model.PurchaseStatus != PurchaseStatus.GiftedAway)
                    {
                        newSeries.Completed = true;
                    }
                }
                var id = _seriesService.Add(newSeries);
                series = _seriesService.GetById(id);
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
            else
                model.CoverUrl = "images\\static\\no-cover.png";

            var volume = _mapper.Map<Volume>(model);

            switch (model.PurchaseStatus)
            {
                case PurchaseStatus.Announced:
                    volume.ReleaseDate = model.ReleaseDate;
                    break;
                case PurchaseStatus.Preordered:
                    volume.ReleaseDate = model.ReleaseDate;
                    volume.PreorderDate = model.PreorderDate;
                    break;
                case PurchaseStatus.Wishlist:
                    break;
                default:
                    volume.PurchaseDate = model.PurchaseDate;
                    break;
            }

            if(model.Status is Status.Dropped or Status.Completed)
            {
                volume.Rating = model.Rating;
            }

            if (model.SingleVolume)
            {
                volume.Number = 1;
            }

            _seriesService.LoadCollection(series, x => x.Volumes);

            if((volume.PurchaseStatus is PurchaseStatus.Bought 
                or PurchaseStatus.Pirated 
                or  PurchaseStatus.Gift 
                or PurchaseStatus.Free) && series.Volumes.Count > 0 && series.Volumes.All(x=>x.Status == Status.Completed))
            {
                volume.Status = Status.InQueue;
            }

            volume.Series = series;
            volume.Authors = _authorService.GetAll(authorList);
            volume.CreationDate = DateTime.Now;
            volume.ModificationDate = DateTime.Now;

            this.Add(volume);

            if(!series.Ongoing && series.Volumes.Count == series.TotalVolumes)
            {
                series.Completed = true;
                ///TODO: fix this
                _seriesService.Update(series);
            }

            return volume.Id;
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

            if (volumeToUpdate.PurchaseStatus is PurchaseStatus.Bought
                or PurchaseStatus.Pirated
                or PurchaseStatus.Gift
                or PurchaseStatus.Free)
            {
                var series = _seriesService.GetById(item.SeriesId);
                _seriesService.LoadCollection(series, x => x.Volumes);

                var volumesExceptCurrent = series.Volumes.Except(new[] { item });

                if (volumesExceptCurrent.Count() > 0 && volumesExceptCurrent.All(x => x.Status == Status.Completed))
                {
                    item.Status = Status.InQueue;
                }

            }


            if (item.CreationDate == default)
                item.CreationDate = DateTime.Now;

            item.ModificationDate = DateTime.Now;

            if (item.SeriesId == 0)
                item.SeriesId = _seriesService.GetByName(item.Series.Name).Id;

            base.Update(item);

            if (item.PurchaseStatus != PurchaseStatus.Announced || item.PurchaseStatus != PurchaseStatus.GiftedAway)
            {
                var series = _seriesService.GetById(item.SeriesId);
                if(!series.Ongoing && series.Volumes.Count == series.TotalVolumes)
                {
                    series.Completed = true;
                    ///TODO: fix this
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
                .Include(x=>x.Series)
                    .ThenInclude(x=>x.Publisher)
                    .ThenInclude(x=>x.Country)
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

            return _mapper.ProjectTo < VolumeViewModel>(filterd).ToList();
        }

        public override string SetNotificationMessage()
        {
            var builder = new StringBuilder();
            if (GetAllEntities().ToList().Any(x => x.Expired()))
            {
                builder.Append("There are volumes that need your attention");
            }

            return builder.ToString();
        }
    }
}
