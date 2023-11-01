using Backend.Models;

namespace Services.ViewModels
{
    public class CountryViewModel : IViewModel<Country>
    {
        public IEnumerable<IdNameView> Publishers { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string FlagPNG { get; set; }
        public string FlagSVG { get; set; }

    }
}
