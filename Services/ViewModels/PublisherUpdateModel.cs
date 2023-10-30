namespace Services.ViewModels
{
    public class PublisherUpdateModel  : IUpdateModel
    {
        public CountryViewModel Country { get; set; }
        public int Id { get; set; }
        public string CountryId { get; set; }
        public string Name { get; set; }

    }
}
