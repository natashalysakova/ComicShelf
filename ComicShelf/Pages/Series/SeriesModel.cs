using ComicShelf.Models;
using System.Diagnostics.CodeAnalysis;

namespace ComicShelf.Pages.SeriesNs
{
    public class SeriesModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? OriginalName { get; set; }
        public bool Ongoing { get; set; }
        public required Models.Enums.Type Type { get; set; }
        public int TotalIssues { get; set; }
        public int HasIssues { get; set; }
        public bool Completed { get; set; }

        public ICollection<string> PublishersIds { get; set; } = new List<string>();

        internal Models.Series ToModel(ComicShelfContext context)
        {
            return new Models.Series()
            {
                Name = Name,
                OriginalName = OriginalName,
                Type = Type,
                Id = Id,
                Ongoing = Ongoing,
                Publishers = context.Publishers.Where(x => PublishersIds.Contains(x.Id.ToString())).ToList(),
                Completed = Completed,
                HasIssues = HasIssues,
                TotalIssues = TotalIssues
            };
        }

        internal void Update(ComicShelfContext context, Models.Series original)
        {
            original.Name = Name;
            original.OriginalName = OriginalName;
            original.Type = Type;
            original.Id = Id;
            original.Ongoing = Ongoing;
            original.Completed = Completed;
            original.HasIssues = HasIssues;
            original.TotalIssues = TotalIssues;
            
            original.Publishers.Clear();
            foreach (var item in context.Publishers.Where(x => PublishersIds.Contains(x.Id.ToString())))
            {
                original.Publishers.Add(item);
            }
        }

        [SetsRequiredMembers]
        public SeriesModel(Models.Series series)
        {
            OriginalName = series.OriginalName;
            Type = series.Type;
            Id = series.Id;
            Ongoing = series.Ongoing;
            Name = series.Name;
            PublishersIds = series.Publishers.Select(x => x.Id.ToString()).ToList();
            HasIssues = series.HasIssues;
            TotalIssues = series.TotalIssues;
            Completed = series.Completed;
        }

        public SeriesModel()
        {

        }
    }
}
