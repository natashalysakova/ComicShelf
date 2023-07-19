using ComicShelf.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using SQLitePCL;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ComicShelf.Pages.SeriesNs
{
    public class SeriesModel 
    {
        public int Id { get; set; }
        [DisplayName("Title")]
        public string Name { get; set; }
        [DisplayName("Original Title")]
        public string? OriginalName { get; set; }
        [DisplayName("Ongoing")]
        public bool Ongoing { get; set; }
        [DisplayName("Type")]
        public required Models.Enums.Type Type { get; set; }

        [DisplayName("Total issues")]
        [RequiredIf(nameof(Ongoing), false, "Provide total issue for finished series")]
        public int? TotalIssues { get; set; }
        [DisplayName("Have issues")]
        public int HasIssues { get; set; }
        [DisplayName("Completed collection")]
        public bool Completed { get; set; }

        [DisplayName("Publishers")]
        public string Publishers { get; set; }



        internal Models.Series ToModel(ComicShelfContext context)
        {
            var splited = Publishers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<Publisher> publisherList = new List<Publisher>();
            foreach (var item in splited)
            {
                var trimmedItem = item.Trim();
                if (string.IsNullOrEmpty(trimmedItem))
                {
                    continue;
                }

                var publisher = context.Publishers.FirstOrDefault(x => x.Name == trimmedItem);
                if (publisher == null)
                {
                    publisher = context.Publishers.Add(new Publisher() { Country = context.UnknownCountry, Name = trimmedItem }).Entity;
                    context.SaveChanges();
                }

                publisherList.Add(publisher);
            }

            return new Models.Series()
            {
                Name = Name,
                OriginalName = OriginalName,
                Type = Type,
                Id = Id,
                Ongoing = Ongoing,
                Publishers = publisherList,
                Completed = Completed,
                HasIssues = HasIssues,
                TotalIssues = TotalIssues.HasValue ? TotalIssues.Value : 0,
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
            original.TotalIssues = TotalIssues.HasValue ? TotalIssues.Value : 0;

            original.Publishers.Clear();

            var splited = Publishers.Split(',');
            var publishers = context.Publishers.Where(x => splited.Contains(x.Name));

            foreach (var item in publishers)
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
            Publishers = string.Join(',', series.Publishers.Select(x => x.Id.ToString()));
            HasIssues = series.HasIssues;
            TotalIssues = series.TotalIssues;
            Completed = series.Completed;
        }

        public SeriesModel()
        {

        }
    }
}
