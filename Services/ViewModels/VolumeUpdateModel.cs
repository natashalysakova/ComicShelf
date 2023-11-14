using Backend.Models;
using Backend.Models.Enums;
using Microsoft.AspNetCore.Http;

namespace Services.ViewModels
{
    public class VolumeUpdateModel : IUpdateModel<Volume>
    {
        public int Id { get; set; }
        public PurchaseStatus PurchaseStatus { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime ReleaseDate { get; set; }
        public DateTime PreorderDate { get; set; }
        public Status Status { get; set; }
        public int Rating { get; set; }
        public IFormFile? CoverFile { get; set; }
        public string CoverUrl { get; set; }

        public IssueUpdateModel[] Issues { get; set; }
        public IssueUpdateModel[] BonusIssues { get; set; }
    }
}
