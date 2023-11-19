using Backend.Models;
using Backend.Models.Enums;

namespace Services.ViewModels
{
    public class VolumeViewModel : IViewModel<Volume>
    {
        public bool Expired { get; internal set; }
        public PurchaseStatus PurchaseStatus { get; internal set; }
        public VolumeType Digitality { get; internal set; }
        public Status Status { get; internal set; }
        public int Rating { get; internal set; }
        public DateTime PurchaseDate { get; internal set; }
        public DateTime? ReleaseDate { get; internal set; }
        public DateTime PreorderDate { get; internal set; }
        public string CoverUrl { get; internal set; }
        public string SeriesName { get; set; }
        public string SeriesOriginalName { get; set; }
        public string SeriesColor { get; set; }
        public int Id { get; set; }
        public bool OneShot { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public IEnumerable<string> Authors { get; set; }
        public string SeriesPublisherName { get; set; }
        public string SeriesPublisherCountryFlag { get; set; }
        public string SeriesPublisherUrl { get; set; }
        public bool SeriesOngoing { get; set; }
        public int SeriesTotalVolumes { get; set; }
        public int SeriesTotalIssues { get; set; }
        public string SeriesType { get; set; }
        public int MalId { get; set; }
        public bool SingleIssue { get; set; }
        public string IssuesRange { get; set; }

        public IssueViewModel[] Issues { get; set; }
        public IssueViewModel[] BonusIssues { get; set; }

        public bool HasError { get; set; }
    }
}
