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

        public SeriesService(ComicShelfContext context, PublishersService service, IMapper mapper) : base(context, mapper)
        {
            _publishersService = service;
        }



        public override int Add(SeriesCreateModel model)
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
                Publisher = _publishersService.GetById(model.Publisher),
                Completed = model.Completed,
                TotalVolumes = model.TotalVolumes.HasValue ? model.TotalVolumes.Value : 0,
                Color = ColorUtility.HexConverter(color),
                ComplimentColor = ColorUtility.HexConverter(complementary)
            };

            this.Add(series);
            return series.Id;
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


        public override void Update(SeriesUpdateModel model)
        {

            var original = dbSet.Include(x => x.Publisher).Single(x => x.Id == model.Id);

            original.Name = model.Name;
            original.OriginalName = model.OriginalName;
            original.Type = model.Type;
            original.Ongoing = model.Ongoing;
            original.Completed = model.Completed;
            original.TotalVolumes = model.TotalVolumes.HasValue ? model.TotalVolumes.Value : 0;
            original.Publisher = _publishersService.GetById(model.Publisher);
            original.Color = model.Color;

            LoadCollection(original, x => x.Volumes);

            if (original.Volumes.Count == original.TotalVolumes)
                original.Completed = true;
            else
                original.Completed = false;



            base.Update(original);
        }
    }
}
