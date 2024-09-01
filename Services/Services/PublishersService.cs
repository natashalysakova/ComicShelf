using AutoMapper;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Services.ViewModels;

namespace Services.Services
{
    public class PublishersService : BasicService<Publisher, PublisherViewModel, PublisherCreateModel, PublisherUpdateModel>
    {
        private const string unknown = "Unknown";

        public PublishersService(ComicShelfContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public PublishersService() : base() { }

        public PublisherViewModel? GetByName(string trimmedItem)
        {
            return _mapper.Map<PublisherViewModel>(dbSet.FirstOrDefault(x => x.Name == trimmedItem));
        }

        public PublisherViewModel? GetWithCountry(int? id)
        {
            return _mapper.Map<PublisherViewModel>(dbSet.Include(x => x.Country).SingleOrDefault(x => x.Id == id));
        }


        public override string SetNotificationMessage()
        {
            if (dbSet.Any(x => x.Country == null))
            {
                return "Some publishers has no country";
            }

            return string.Empty;
        }

        internal Publisher GetUnknownn()
        {
            return dbSet.Single(x => x.Name == unknown);
        }

        public override IEnumerable<PublisherViewModel> GetAll()
        {
            return GetAllEntities()
                .Include(x => x.Series)
                .Include(x => x.Country)
                .Select(x => _mapper.Map<PublisherViewModel>(x));
        }
    }
}
