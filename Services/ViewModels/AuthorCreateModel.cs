using Backend.Models.Enums;

namespace Services.ViewModels
{
    public class AuthorCreateModel : ICreateModel
    {
        public string Name { get; internal set; }
        public Roles Roles { get; internal set; }
    }
}
