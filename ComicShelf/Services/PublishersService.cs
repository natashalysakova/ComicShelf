using ComicShelf.Models;
using ComicShelf.Pages.Countries;
using ComicShelf.Pages.Publishers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace ComicShelf.Services
{
    public class PublishersService : BasicService<Publisher>
    {
        public PublishersService(ComicShelfContext context) : base(context) 
        {
        }

        internal Publisher? GetByName(string trimmedItem)
        {
            return dbSet.FirstOrDefault(x => x.Name == trimmedItem);
        }

        //public void Add(PublisherModel model)
        //{
        //    var publisher = new Publisher
        //    {
        //        Name = model.Name,
        //        Country = context.Set<Country>().Single(x=>x.Id == int.Parse(model.CountryId))
        //    };

        //    this.Add(publisher);
        //}

        //public void Update(PublisherModel model)
        //{
        //    var original = Get(model.Id);

        //    original.Country = context.Set<Country>().Single(x => x.Id == int.Parse(model.CountryId));
        //    original.Name = model.Name;

        //    this.Update(original);
        //}

        internal Publisher? GetWithCountry(int? id)
        {
            return context.Set<Publisher>().Include(x => x.Country).SingleOrDefault(x => x.Id == id);
        }

        public override void Update(Publisher country)
        {
            base.Update(country);
        }


        public override string SetNotificationMessage()
        {
            if(dbSet.Any(x=>x.Country == null))
            {
                return "Some publishers has no country";
            }

            return string.Empty;
        }
    }

    enum PublisherSort
    {
        Name, Country
    }
}
