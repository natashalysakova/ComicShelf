namespace Services.ViewModels
{
    public class FilterViewModel : IViewModel
    {
        public string Group { get; internal set; }
        public int DisplayOrder { get; internal set; }
        public string Name { get; internal set; }
        public string Json { get; internal set; }
        public int Id { get; internal set; }
        public bool Selected { get; set; } = false;
    }
}
