using Backend.Models;
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
        public DateTime? PurchaseDate { get; set; }
        //public string Cover { get; set; }

        [DisplayName("Release")]
        public DateTime? ReleaseDate { get; set; }
        public DateTime? PreorderDate { get; set; }
        public IFormFile? CoverFile { get; set; }
        public string? CoverUrl { get; set; }

        public string[] Authors { get; set; }
        public VolumeType Digitality { get; set; }
        public string SeriesName { get; set; }
        public int NumberOfIssues { get; set; }
        public int NumberOfBonusIssues { get; set; }
        public VolumeItemType VolumeType { get; set; }
    }

    public enum VolumeItemType
    {
        All,
        Volume,
        OneShot,
        Issue
    }
}
