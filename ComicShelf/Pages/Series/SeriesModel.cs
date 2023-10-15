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
        [Required]
        public Models.Enums.Type Type { get; set; }

        [DisplayName("Total issues")]
        [RequiredIf(nameof(Ongoing), false, "Provide total issue for finished series")]
        public int? TotalVolumes { get; set; }
        [DisplayName("Have issues")]
        public int HasIssues { get; set; }
        [DisplayName("Completed collection")]
        public bool Completed { get; set; }

        [DisplayName("Publisher")]
        public int Publisher { get; set; }
        [DisplayName("Color")]
        public string Color { get; set; }
        public int VolumeCount { get; set; }


        public SeriesModel(Models.Series series)
        {
            OriginalName = series.OriginalName;
            Type = series.Type;
            Id = series.Id;
            Ongoing = series.Ongoing;
            Name = series.Name;
            Publisher = series.Publisher is null ? 0 : series.Publisher.Id;
            TotalVolumes = series.TotalVolumes;
            Completed = series.Completed;
            Color = series.Color;
            VolumeCount = series.Volumes.Count;
        }

        public SeriesModel()
        {

        }
    }
}
