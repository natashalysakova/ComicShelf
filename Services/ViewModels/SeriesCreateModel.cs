using Backend.Models;

namespace Services.ViewModels
{
    public class SeriesCreateModel : ICreateModel<Series>
    {
        public string Name { get; set; }
        public Backend.Models.Enums.Type Type { get; set; }
        public int TotalVolumes { get; set; }
        public bool Completed { get; set; }
        public bool Ongoing { get; set; }
        public string? PublisherName { get; set; }
        public string? OriginalName { get; set; }
    }
}
