using Backend.Models;

namespace Services.ViewModels
{
    public class SeriesViewModel : IViewModel<Series>
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public bool Ongoing { get; set; }
        public int VolumesCount { get; set; }
        public bool Completed { get; set; }
        public int TotalVolumes { get; set; }
        public string OriginalName { get; set; }
        public string Color { get; set; }
        public int MalId { get; set; }

    }
}
