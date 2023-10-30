namespace Services.ViewModels
{
    public class PublisherViewModel : IViewModel
    {
        public string CountryId { get; set; }
        public int SeriesCount { get; set; }
        public object Name { get; set; }
        public string Id { get; set; }
        public string CountryName { get; set; }
        public string CountryFlagPNG { get; set; }
        public IEnumerable<SeriesViewModel> Series { get; set; }

    }
}
