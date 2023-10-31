using Backend.Models;
using Backend.Models.Enums;

namespace Services.ViewModels
{
    public class AuthorViewModel : IViewModel
    {
        public string Name { get; internal set; }
        public int Id { get; internal set; }
        public Roles Roles { get; set; }
        public IEnumerable<string> SeriesNames { get; set; }
    }
}
