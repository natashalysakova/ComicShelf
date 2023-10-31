using Backend.Models.Enums;

namespace Services.ViewModels
{
    public class AuthorUpdateModel : IUpdateModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Roles Roles { get; set; }
    }
}
