using AutoMapper;
using Backend.Models;
using Services.ViewModels;

namespace ComicShelf.Profiles
{
    public class FiltersProfile : Profile
    {
        public FiltersProfile()
        {
            CreateMap<FilterCreateModel, Filter>(MemberList.Source);
            CreateMap<FilterUpdateModel, Filter>(MemberList.Source).ReverseMap();
            CreateMap<Filter,FilterViewModel>()
                .ForMember(x=>x.Selected, act => { act.Ignore(); });

        }
    }
}
