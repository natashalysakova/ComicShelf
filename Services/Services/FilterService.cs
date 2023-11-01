using AutoMapper;
using Backend.Models;
using Services.ViewModels;

namespace Services.Services
{
    public class FilterService : BasicService<Filter, FilterViewModel, FilterCreateModel, FilterUpdateModel>
    {
        public const string STANDART = "standart";
        public const string CUSTOM = "custom";

        public FilterService(ComicShelfContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public IEnumerable<IGrouping<string, FilterViewModel>> GetAllForView(string selectedFilter = "")
        {
            var filters = GetAll();

            var filterToSelect = filters.FirstOrDefault(filter => filter.Name == selectedFilter);
            if (filterToSelect != null)
                filterToSelect.Selected = true;

            return filters.GroupBy(x => x.Group).OrderByDescending(x => x.Key);
        }

        public override int Add(FilterCreateModel item)
        {
            var model = _mapper.Map<Filter>(item);

            if (string.IsNullOrEmpty(model.Group))
            {
                model.Group = CUSTOM;
            }

            base.Add(model);
            return model.Id;
        }

        public IEnumerable<int> AddRange(IEnumerable<FilterCreateModel> items)
        {
            foreach (var item in items)
            {
                if (string.IsNullOrEmpty(item.Group))
                {
                    item.Group = CUSTOM;
                }
            }

            return base.AddRange(items.Select(x => _mapper.Map<Filter>(x)));
        }

        public override string SetNotificationMessage()
        {
            return string.Empty;
        }
    }
}
