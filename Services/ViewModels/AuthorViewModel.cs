using Backend.Models;

namespace Services.ViewModels
{
    public class AuthorViewModel : IViewModel<Author>
    {
        public string Name { get; internal set; }
        public int Id { get; internal set; }
    }
}
