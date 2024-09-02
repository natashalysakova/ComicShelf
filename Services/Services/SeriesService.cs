using AutoMapper;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Services.ViewModels;
using System.Text;

namespace Services.Services
{
    public class SeriesService : BasicService<Series, SeriesViewModel, SeriesCreateModel, SeriesUpdateModel>
    {
        private readonly PublishersService _publishersService;
        private readonly CountryService _countryService;

        public SeriesService(ComicShelfContext context, PublishersService service, CountryService countryService, IMapper mapper) : base(context, mapper)
        {
            _publishersService = service;
            _countryService = countryService;
        }



        public override int Add(SeriesCreateModel model)
        {
            var color = ColorUtility.GetRandomColor(minSaturation: 50, minValue: 50);
            var complementary = ColorUtility.GetOppositeColor(color);
            var entity = _mapper.Map<Series>(model);

            if (model.PublisherName == null)
            {
                entity.Publisher = _publishersService.GetUnknownn();
            }
            else
            {
                var publisher = _publishersService.GetByName(model.PublisherName);
                if (publisher == null)
                {
                    entity.PublisherId = _publishersService.Add(new PublisherCreateModel() { Name = model.PublisherName, Url = string.Empty, CountryId = _countryService.GetUnknownn().Id });
                }
                else
                {
                    entity.PublisherId = publisher.Id;
                }
            }

            entity.Color = ColorUtility.HexConverter(color);
            entity.ComplimentColor = ColorUtility.HexConverter(complementary);

            return Add(entity);
        }

        internal Series GetByName(string selectedSeries)
        {
            return dbSet.Where(x => x.Name == selectedSeries).SingleOrDefault();
        }

        public override string SetNotificationMessage()
        {
            StringBuilder builder = new StringBuilder();

            if (dbSet.Any(x => x.Publisher.Id == 0
            || !x.Volumes.Any()
            || (!x.Ongoing && x.TotalVolumes == 0)
            || (!x.Ongoing && x.TotalIssues == 0)
            || (x.MalId == 0 && x.Type == Backend.Models.Enums.Type.Manga)))
            {
                return "You have not filled data for series";
            }

            return string.Empty;
        }

        public override IQueryable<Series> GetAllEntities(bool tracking = false)
        {
            return base.GetAllEntities(tracking).Include(x => x.Publisher).Include(x=>x.Volumes).ThenInclude(x=>x.Issues);
        }

        public override void Update(SeriesUpdateModel item)
        {
            var series = GetById(item.Id);
            LoadCollection(series, x => x.Volumes);

            if (item.TotalVolumes == 1 && !item.Ongoing)
            {
                var volume = series.Volumes.SingleOrDefault();
                if (volume != null)
                {
                    volume.OneShot = true;
                }
            }
            else
            {
                foreach (var vol in series.Volumes)
                {
                    vol.OneShot = false;
                }
            }
            SaveChanges();

            base.Update(item);
        }

        public string? GetSeriesAuthor(string series)
        {
            var entity = this.dbSet.Include(x=>x.Volumes).ThenInclude(x=>x.Authors).SingleOrDefault(x => x.Name == series);

            if(entity == null)
                return null;

            var lastVolume = entity.Volumes.SingleOrDefault(x => x.Number == entity.Volumes.Max(x=>x.Number));
            if (lastVolume == null)
                return null;

            return string.Join(',', lastVolume.Authors.Select(x=>x.Name));


        }
    }
}
