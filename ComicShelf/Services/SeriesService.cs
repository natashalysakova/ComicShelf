using ComicShelf.Models;
using ComicShelf.Pages.SeriesNs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Text;

namespace ComicShelf.Services
{
    public class SeriesService : BasicService<Series>
    {
        private readonly PublishersService _publishersService;
        CountryService _countryService;

        public SeriesService(ComicShelfContext context, PublishersService service, CountryService countryService) : base(context)
        {
            _publishersService = service;
            _countryService = countryService;
        }

        public override IQueryable<Series> GetAll()
        {
            return base.GetAll().Include(x=>x.Volumes).Include(x=>x.Publisher);
        }

        public Series? GetWithPublishers(int? id)
        {
            return dbSet.Include(x => x.Publisher).SingleOrDefault(x => x.Id == id);
        }

        public override void Add(Series item)
        {
            var color = ColorUtility.GetRandomColor(minSaturation: 50, minValue: 50);
            var complementary = ColorUtility.GetOppositeColor(color);
            item.Color = ColorUtility.HexConverter(color);
            item.ComplimentColor = ColorUtility.HexConverter(complementary);
            if (item.Publisher == null)
            {
                item.Publisher = _publishersService.GetByName("Unknown");
            }
            base.Add(item);
        }

        public void Add(SeriesModel model)
        {
            var color = ColorUtility.GetRandomColor(minSaturation: 50, minValue: 50);
            var complementary = ColorUtility.GetOppositeColor(color);

            var series = new Series()
            {
                Name = model.Name,
                OriginalName = model.OriginalName,
                Type = model.Type,
                Id = model.Id,
                Ongoing = model.Ongoing,
                Publisher = _publishersService.Get(model.Publisher),
                Completed = model.Completed,
                TotalVolumes = model.TotalVolumes.HasValue ? model.TotalVolumes.Value : 0,
                Color = ColorUtility.HexConverter(color),
                ComplimentColor = ColorUtility.HexConverter(complementary)
            };

            this.Add(series);
        }

        public void Update(SeriesModel model)
        {
            var original = dbSet.Include(x => x.Publisher).Single(x => x.Id == model.Id);

            original.Name = model.Name;
            original.OriginalName = model.OriginalName;
            original.Type = model.Type;
            original.Id = model.Id;
            original.Ongoing = model.Ongoing;
            original.Completed = model.Completed;
            original.TotalVolumes = model.TotalVolumes.HasValue ? model.TotalVolumes.Value : 0;
            original.Publisher = _publishersService.Get(model.Publisher);
            original.Color = model.Color;

            LoadCollection(original, x => x.Volumes);

            if (original.Volumes.Count == original.TotalVolumes)
                original.Completed = true;
            else
                original.Completed = false;

            //original.Publishers.Clear();

            //var splited = model.Publishers.Split(',');
            //foreach (var item in splited.Distinct())
            //{
            //    var trimmedItem = item.Trim();

            //    if (string.IsNullOrEmpty(trimmedItem))
            //        continue;

            //    var publisher = _publishersService.GetByName(trimmedItem);
            //    if (publisher == null)
            //    {
            //        publisher = new Models.Publisher() { Country = _countryService.UnknownCountry, Name = trimmedItem };
            //        _publishersService.Add(publisher);
            //    }

            //    original.Publishers.Add(publisher);
            //}

            this.Update(original);
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
            || (!x.Ongoing && x.TotalVolumes == 0)))
            {
                return "You have not filled data for series";
            }

            return string.Empty;
        }
    }

    enum SeriesControllerSort
    {
        Name, Publisher,
    }
}
