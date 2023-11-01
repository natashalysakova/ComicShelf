using Backend.Models;
using Backend.Models.Enums;

namespace Services.ViewModels
{
    public class AuthorCreateModel : ICreateModel<Author>
    {
        public string Name { get; set; }
        public Roles Roles { get; set; }
    }
}
