namespace Services.ViewModels
{
    public class PublisherCreateModel : ICreateModel
    {
        public IViewModel Country { get; set; }
        public string Name { get; set; }
    }
}
