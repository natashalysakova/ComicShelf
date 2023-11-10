using Backend.Models;

namespace Services.ViewModels
{
    public class PublisherViewModel : IViewModel<Publisher>
    {
        public int CountryId { get; set; }
        public int SeriesCount { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
        public string CountryName { get; set; }
        public string CountryFlagPNG { get; set; }
        public IEnumerable<IdNameView> Series { get; set; }
        public string Url { get; set; }

    }
}
