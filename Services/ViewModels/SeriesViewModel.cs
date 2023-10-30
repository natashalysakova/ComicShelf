namespace Services.ViewModels
{
    public class SeriesViewModel : IViewModel
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public bool Ongoing { get; set; }
        public int VolumesCount { get; set; }
        public bool Completed { get; set; }
        public int TotalVolumes { get; set; }
        public string OriginalName { get; set; }
    }
}
