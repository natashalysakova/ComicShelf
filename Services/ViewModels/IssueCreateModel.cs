using Backend.Models;

namespace Services.ViewModels
{
    public class IssueCreateModel : ICreateModel<Issue>
    {
        public string Name { get; set; }
        public int Number { get; set; }
        public string Type { get; set; }
    }
}