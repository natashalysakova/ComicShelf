﻿using Backend.Models;
using Backend.Models.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Services.ViewModels
{
    public class VolumeCreateModel : ICreateModel<Volume>
    {
        public int Number { get; set; }

        [Required]
        public string Title { get; set; }
        public Status Status { get; set; }
        public int Rating { get; set; }
        public PurchaseStatus PurchaseStatus { get; set; }
        [DisplayName("Added to librarary")]
        public DateTime? PurchaseDate { get; set; } = DateTime.Today;
        //public string Cover { get; set; }

        [DisplayName("Release")]
        public DateTime? ReleaseDate { get; set; } = DateTime.Today;
        public DateTime? PreorderDate { get; set; } = DateTime.Today;
        public IFormFile? CoverFile { get; set; }
        public string? CoverUrl { get; set; }
        public string? CoverToDownload { get; set; }

        public string[] Authors { get; set; }
        public VolumeType Digitality { get; set; }
        public string SeriesName { get; set; }
        public int NumberOfIssues { get; set; } = 0;
        public int NumberOfBonusIssues { get; set; } = 0;
        public VolumeItemType VolumeType { get; set; }

        public string? PublisherName { get; set; }
        public int TotalVolumes { get; set; }
        public string? SeriesStatus { get; set; }
        public string? ISBN { get; set; }
        public string? SeriesOriginalName { get; set; }
    }

    public enum VolumeItemType
    {
        All,
        Volume,
        OneShot,
        Issue
    }
}
