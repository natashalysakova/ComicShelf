using Backend.Models;

namespace Services.ViewModels
{
    public class FilterCreateModel : ICreateModel<Filter>
    {
        public string Group { get; set; }
        public string Name { get; set; }
        public string Json { get; set; }
    }
}
