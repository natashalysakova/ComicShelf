using Backend.Models;

namespace Services.ViewModels
{
    public class FilterUpdateModel : IUpdateModel<Filter>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
    }
}
