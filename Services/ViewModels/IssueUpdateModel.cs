using Backend.Models;

namespace Services.ViewModels
{
    public class IssueUpdateModel : IUpdateModel<Issue>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
    }
}