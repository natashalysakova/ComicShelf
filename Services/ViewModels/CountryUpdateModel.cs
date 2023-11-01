using Backend.Models;

namespace Services.ViewModels
{
    public class CountryUpdateModel : IUpdateModel<Country>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FlagPNG { get; set; }
        public string FlagSVG { get; set; }
        public string CountryCode { get; set; }
    }
}
