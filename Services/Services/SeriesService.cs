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


        public override IEnumerable<SeriesUpdateModel> GetAllForEdit()
        {
            return _mapper.ProjectTo<SeriesUpdateModel>(GetAllEntities().Include(x => x.Publisher));
        }

        public override void Update(SeriesUpdateModel model)
        {
            var entry = _mapper.Map<Series>(model); 


            //var original = dbSet.Include(x => x.Publisher).Single(x => x.Id == model.Id);

            //original.Name = model.Name;
            //original.OriginalName = model.OriginalName;
            //original.Type = model.Type;
            //original.Ongoing = model.Ongoing;
            //original.Completed = model.Completed;
            //original.TotalVolumes = model.TotalVolumes.HasValue ? model.TotalVolumes.Value : 0;
            //original.PublisherId = model.PublisherId;
            //original.Color = model.Color;

            //LoadCollection(entry, x => x.Volumes);

            //if (entry.Volumes.Count == entry.TotalVolumes)
            //    entry.Completed = true;
            //else
            //    entry.Completed = false;



            base.Update(entry);
        }
    }
}
