using AutoMapper;
using Backend.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Services.ViewModels;

namespace Services.Services
{
    public class CountryService : BasicService<Country, CountryViewModel, CountryCreateModel, CountryUpdateModel>
    {
        public CountryService(ComicShelfContext context, IMapper mapper) : base(context, mapper)
        {
        }

        SelectListGroup popularGroup = new SelectListGroup() { Name = "Popular" };
        SelectListGroup otherGroup = new SelectListGroup() { Name = "Other" };
        private CountryViewModel UnknownCountry { get => base.GetAll().Single(x => x.Name == "Unknown"); }

        public IEnumerable<SelectListItem> GetCountriesForView()
        {
            var allExceptUnknown = GetAllEntities().Include(x => x.Publishers).ToList();
            var toRemove = allExceptUnknown.Single(x => x.Id == UnknownCountry.Id);
            allExceptUnknown.Remove(toRemove);

            var selectListItems = allExceptUnknown.OrderByDescending(x => x.Publishers.Count()).ThenBy(x => x.Name).Select(x => new SelectListItem()
            {
                Text = $"{x.Name}",
                Value = x.Id.ToString(),
                Group = x.Publishers.Any() ? popularGroup : otherGroup
            })
            .OrderByDescending(x => x.Group.Name).ToList();

            return selectListItems;


        }

        public override string SetNotificationMessage()
        {
            return string.Empty;
        }

        private const string unknown = "Unknown";
        internal Country GetUnknownn()
        {
            return dbSet.Single(x => x.Name == unknown);
        }
    }

    enum CountrySort
    {
        Name
    }
}
