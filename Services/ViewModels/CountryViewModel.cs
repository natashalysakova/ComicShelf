namespace Services.ViewModels
{
    public class CountryViewModel : IViewModel
    {
        public IEnumerable<string> Publishers { get; set; }
        public IViewModel Id { get; set; }
        public string Name { get; set; }
        public string FlagPNG { get; set; }
        public string FlagSVG { get; set; }

    }
}
