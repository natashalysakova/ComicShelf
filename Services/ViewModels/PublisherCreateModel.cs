using Backend.Models;
using System.ComponentModel;

namespace Services.ViewModels
{
    public class PublisherCreateModel : ICreateModel<Publisher>
    {
        [DisplayName("Country")]
        public int CountryId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }

    }
}
