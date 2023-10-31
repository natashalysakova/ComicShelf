namespace Services.ViewModels
{
    public class CountryCreateModel : ICreateModel
    {
        public string Name { get; set; }
        public string FlagPNG { get; set; }
        public string FlagSVG { get; set; }
        public string CountryCode { get; set; }
    }
}
