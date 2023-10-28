using ComicShelf.Models;

namespace ComicShelf.Services
{
    public class FilterService : BasicService<Filter>
    {
        public const string STANDART = "standart";
        public const string CUSTOM = "custom";


        public FilterService(ComicShelfContext context) : base(context)
        {
        }

        public IEnumerable<dynamic> GetAllForView(string selectedFilter = "")
        {
            return GetAll().GroupBy(x => x.Group).OrderByDescending(x=>x.Key)
                .Select(group => new
                {
                    name = group.Key,
                    items = group
                    .OrderBy(x => x.DisplayOrder)
                    .ThenBy(x => x.Name)
                    .Select(x => new { id = x.Id, name = x.Name, selected = x.Name == selectedFilter, json = x.Json }).ToList()
                });
        }

        public override void Add(Filter item)
        {
            if(string.IsNullOrEmpty(item.Group))
            {
                item.Group = CUSTOM;
            }

            base.Add(item);
        }

        public override void AddRange(IEnumerable<Filter> items)
        {
            foreach(Filter item in items)
            {
                if (string.IsNullOrEmpty(item.Group))
                {
                    item.Group = CUSTOM;
                }
            }

            base.AddRange(items);
        }

        public override string SetNotificationMessage()
        {
            return string.Empty;
        }
    }
}
