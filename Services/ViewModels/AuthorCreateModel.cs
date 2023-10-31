using Backend.Models.Enums;

namespace Services.ViewModels
{
    public class AuthorCreateModel : ICreateModel
    {
        public string Name { get; set; }
        public Roles Roles { get; set; }
    }
}
