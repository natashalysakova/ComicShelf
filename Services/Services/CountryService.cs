using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Backend.Models;
using Services.ViewModels;
using AutoMapper;

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

        public IQueryable<CountryViewModel> GetAll()
        {
            return base.GetAll().Select(x => _mapper.Map<CountryViewModel>(x));
        }

        public IEnumerable<SelectListItem> GetCountriesForView()
        {
            var allExceptUnknown = base.GetAll().Include(x=>x.Publishers).ToList();
            var toRemove = allExceptUnknown.Single(x => x.Id == UnknownCountry.Id);
            allExceptUnknown.Remove(toRemove);

            var selectListItems = allExceptUnknown.OrderByDescending(x=>x.Publishers.Count()).ThenBy(x=>x.Name).Select(x => new SelectListItem()
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

    }

    enum CountrySort
    {
        Name
    }
}
