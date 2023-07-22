using ComicShelf.Models;
using ComicShelf.Pages.SeriesNs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


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

            var series = new Series()
            {
                Name = model.Name,
                OriginalName = model.OriginalName,
                Type = model.Type,
                Id = model.Id,
                Ongoing = model.Ongoing,
                Publishers = publisherList,
                Completed = model.Completed,
                HasIssues = model.HasIssues,
                TotalIssues = model.TotalIssues.HasValue ? model.TotalIssues.Value : 0,
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
            original.HasIssues = model.HasIssues;
            original.TotalIssues = model.TotalIssues.HasValue ? model.TotalIssues.Value : 0;

            original.Publishers.Clear();

            var splited = model.Publishers.Split(',');
            foreach (var item in splited.Distinct())
            {
                var trimmedItem = item.Trim();
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
    }

    enum SeriesControllerSort
    {
        Name, Publisher, 
    }
}
