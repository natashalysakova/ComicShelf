using AutoMapper;
using Backend.Models;
using Services.ViewModels;

namespace Services.Services
{
    internal class IssueService : BasicService<Issue, IssueViewModel, IssueCreateModel, IssueUpdateModel>
    {
        public IssueService(ComicShelfContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public override string SetNotificationMessage()
        {
            throw new NotImplementedException();
        }
    }
}