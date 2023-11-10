using AutoMapper;
using Backend.Models;
using Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Profiles
{
    public  class IssueProfile : Profile
    {
        public IssueProfile()
        {
            CreateMap<IssueCreateModel, Issue>(MemberList.Source)
                .ForSourceMember(x=>x.Type, act=>act.DoNotValidate());
            CreateMap<IssueUpdateModel, Issue>(MemberList.Source).ReverseMap();

            CreateMap<Issue, IdNameView>();
            CreateMap<Issue, IssueViewModel>()
                .ForMember(x=>x.Type, act=> act.MapFrom(x=>x.GetType().Name));
        }
    }
}
