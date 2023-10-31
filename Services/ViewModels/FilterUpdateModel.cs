namespace Services.ViewModels
{
    public class FilterUpdateModel : IUpdateModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
    }
}
