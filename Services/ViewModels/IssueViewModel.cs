using Backend.Models;

namespace Services.ViewModels
{
    public class IssueViewModel : IViewModel<Issue>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
        public string Type { get; set; }
    }
}