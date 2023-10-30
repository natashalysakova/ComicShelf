using AutoMapper;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Services.ViewModels;

namespace Services.Services
{
    public class PublishersService : BasicService<Publisher, PublisherViewModel, PublisherCreateModel, PublisherUpdateModel>
    {
        public PublishersService(ComicShelfContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public IQueryable<PublisherViewModel> GetAll()
        {
            return base.GetAll().Select(x => _mapper.Map<PublisherViewModel>(x));
        }

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
    }
}
