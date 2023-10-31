namespace Services.ViewModels
{
    public class PublisherViewModel : IViewModel
    {
        public int CountryId { get; set; }
        public int SeriesCount { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
        public string CountryName { get; set; }
        public string CountryFlagPNG { get; set; }
        public IEnumerable<string> Series { get; set; }

    }
}
