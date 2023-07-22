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

        [SetsRequiredMembers]
        public SeriesModel(Models.Series series)
        {
            OriginalName = series.OriginalName;
            Type = series.Type;
            Id = series.Id;
            Ongoing = series.Ongoing;
            Name = series.Name;
            Publishers = string.Join(',', series.Publishers.Select(x => x.Name.ToString()));
            HasIssues = series.HasIssues;
            TotalIssues = series.TotalIssues;
            Completed = series.Completed;
        }

        public SeriesModel()
        {

        }
    }
}
