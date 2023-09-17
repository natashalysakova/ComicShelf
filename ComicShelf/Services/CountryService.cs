using ComicShelf.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ComicShelf.Services
{
    public class CountryService : BasicService<Country>
    {
        public CountryService(ComicShelfContext context) : base(context)
        {
        }

        SelectListGroup popularGroup = new SelectListGroup() { Name = "Popular" };
        SelectListGroup otherGroup = new SelectListGroup() { Name = "Other" };



        public Country UnknownCountry { get => dbSet.Single(x => x.Name == "Unknown"); }

        public IEnumerable<SelectListItem> GetCountriesForView()
        {
            var allExceptUnknown = GetAll().Include(x=>x.Publishers).ToList();
            var toRemove = allExceptUnknown.Single(x => x.Id == UnknownCountry.Id);
            allExceptUnknown.Remove(toRemove);

            var selectListItems = allExceptUnknown.OrderByDescending(x=>x.Publishers.Count).ThenBy(x=>x.Name).Select(x => new SelectListItem()
            {
                Text = $"{x.Name}",
                Value = x.Id.ToString(),
                Group = x.Publishers.Any() ? popularGroup : otherGroup
            })
            .OrderByDescending(x => x.Group.Name).ToList();

            return selectListItems;


        }


    }

    enum CountrySort
    {
        Name
    }
}
