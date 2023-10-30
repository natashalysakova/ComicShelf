using Backend.Models;

namespace Services.ViewModels
{
    public class AuthorViewModel : IViewModel
    {
        public string Name { get; internal set; }
        public string Id { get; internal set; }
        public string Roles { get; set; }
        public string SeriesNames { get; set; }
    }
}
