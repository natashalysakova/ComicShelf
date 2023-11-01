using AutoMapper;
using Backend.Models;
using Services.ViewModels;

namespace Services.Profiles
{
    public class FiltersProfile : Profile
    {
        public FiltersProfile()
        {
            CreateMap<FilterCreateModel, Filter>(MemberList.Source);
            CreateMap<FilterUpdateModel, Filter>(MemberList.Source).ReverseMap();

            CreateMap<Filter, IdNameView>();
            CreateMap<Filter, FilterViewModel>()
                .ForMember(x => x.Selected, act => { act.Ignore(); });


        }
    }
}
