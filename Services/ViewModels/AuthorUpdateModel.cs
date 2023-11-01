using Backend.Models;
using Backend.Models.Enums;

namespace Services.ViewModels
{
    public class AuthorUpdateModel : IUpdateModel<Author>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Roles Roles { get; set; }
        public bool HasError { get; set; }
        public IEnumerable<IdNameView> Series { get; set; }

    }
}
