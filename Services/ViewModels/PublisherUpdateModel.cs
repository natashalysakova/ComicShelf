using Backend.Models;

namespace Services.ViewModels
{
    public class PublisherUpdateModel : IUpdateModel<Publisher>
    {
        public int Id { get; set; }
        public int CountryId { get; set; }
        public string Name { get; set; }

    }
}
