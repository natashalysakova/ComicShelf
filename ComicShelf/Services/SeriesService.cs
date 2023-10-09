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


        public Series? GetWithPublishers(int? id)
        {
            return dbSet.Include(x => x.Publishers).SingleOrDefault(x => x.Id == id);
        }

        public override void Add(Series item)
        {
            var color = ColorUtility.GetRandomColor(minSaturation: 50, minValue: 50);
            var complementary = ColorUtility.GetOppositeColor(color);
            item.Color = ColorUtility.HexConverter(color);
            item.ComplimentColor = ColorUtility.HexConverter(complementary);

            base.Add(item);
        }

        public void Add(SeriesModel model)
        {
            var splited = model.Publishers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<Publisher> publisherList = new List<Publisher>();
            foreach (var item in splited)
            {
                var trimmedItem = item.Trim();
                if (string.IsNullOrEmpty(trimmedItem))
                {
                    continue;
                }

                var publisher = _publishersService.GetByName(trimmedItem);
                if (publisher == null)
                {
                    publisher = new Publisher() { Country = _countryService.UnknownCountry, Name = trimmedItem };
                    _publishersService.Add(publisher);
                }

                publisherList.Add(publisher);
            }

            var color = ColorUtility.GetRandomColor(minSaturation: 50, minValue: 50);
            var complementary = ColorUtility.GetOppositeColor(color);

            var series = new Series()
            {
                Name = model.Name,
                OriginalName = model.OriginalName,
                Type = model.Type,
                Id = model.Id,
                Ongoing = model.Ongoing,
                Publishers = publisherList,
                Completed = model.Completed,
                TotalVolumes = model.TotalVolumes.HasValue ? model.TotalVolumes.Value : 0,
                Color = ColorUtility.HexConverter(color),
                ComplimentColor = ColorUtility.HexConverter(complementary)
            };

            this.Add(series);
        }

        public void Update(SeriesModel model)
        {
            var original = dbSet.Include(x => x.Publishers).Single(x => x.Id == model.Id);

            original.Name = model.Name;
            original.OriginalName = model.OriginalName;
            original.Type = model.Type;
            original.Id = model.Id;
            original.Ongoing = model.Ongoing;
            original.Completed = model.Completed;
            original.TotalVolumes = model.TotalVolumes.HasValue ? model.TotalVolumes.Value : 0;

            original.Publishers.Clear();

            var splited = model.Publishers.Split(',');
            foreach (var item in splited.Distinct())
            {
                var trimmedItem = item.Trim();

                if (string.IsNullOrEmpty(trimmedItem))
                    continue;

                var publisher = _publishersService.GetByName(trimmedItem);
                if (publisher == null)
                {
                    publisher = new Publisher() { Country = _countryService.UnknownCountry, Name = trimmedItem };
                    _publishersService.Add(publisher);
                }

                original.Publishers.Add(publisher);
            }

            this.Update(original);
        }

        internal Series GetByName(string selectedSeries)
        {
            return dbSet.Where(x => x.Name == selectedSeries).SingleOrDefault();
        }

        public override string SetNotificationMessage()
        {
            StringBuilder builder = new StringBuilder();

            if (dbSet.Any(x => !x.Publishers.Any()  
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
