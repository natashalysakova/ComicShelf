using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Http;
using Backend.Models.Enums;

namespace Services.ViewModels
{
    public class VolumeCreateModel :  ICreateModel
    {
        public int Number { get; set; }

        [Required]
        public string Title { get; set; }
        public string Series { get; set; }
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
        public string[] Authors { get; set; }
        public int Issues { get; set; }
        public VolumeType Digitality { get; set; }
        public bool SingleVolume { get; set; }

    }
}
