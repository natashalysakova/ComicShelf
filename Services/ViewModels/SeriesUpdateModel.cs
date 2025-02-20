﻿using Backend.Models;
using Services.Utilities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Services.ViewModels
{
    public class SeriesUpdateModel : IUpdateModel<Series>
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
        public Backend.Models.Enums.Type Type { get; set; }

        [DisplayName("Total issues")]
        [RequiredIf(nameof(Ongoing), false, "Provide total of volumes for finished series")]
        public int? TotalVolumes { get; set; }

        [RequiredIf(nameof(Ongoing), false, "Provide total of issues for finished series")]
        public int? TotalIssues { get; set; }

        [DisplayName("Completed collection")]
        public bool Completed { get; set; }

        [DisplayName("Publisher")]
        public int PublisherId { get; set; }
        [DisplayName("Color")]
        public string Color { get; set; }

        public int VolumeCount { get; set; }
        public int IssueCount { get; set; }
        public bool HasError { get; set; }
        public int MalId { get; set; }

    }
}
