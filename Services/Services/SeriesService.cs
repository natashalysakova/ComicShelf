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
            var entity = _mapper.Map<Series>(model);
            entity.Publisher = _publishersService.GetUnknownn();
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
            || (!x.Ongoing && x.TotalVolumes == 0)))
            {
                return "You have not filled data for series";
            }

            return string.Empty;
        }

        protected override IQueryable<Series> GetAllEntities(bool tracking = false)
        {
            return base.GetAllEntities(tracking).Include(x => x.Publisher);
        }
    }
}
