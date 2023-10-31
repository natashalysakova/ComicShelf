using System.ComponentModel;

namespace Services.ViewModels
{
    public class PublisherCreateModel : ICreateModel
    {
        [DisplayName("Country")]
        public int CountryId { get; set; }
        public string Name { get; set; }
    }
}
