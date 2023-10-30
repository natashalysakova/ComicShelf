namespace Services.ViewModels
{
    public class FilterCreateModel : ICreateModel
    {
        public string? Group { get; internal set; }
        public string Name { get; set; }
        public string Json { get; set; }
    }
}
