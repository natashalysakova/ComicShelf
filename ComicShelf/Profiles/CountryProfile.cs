using AutoMapper;
using Backend.Models;
using Services.ViewModels;

namespace ComicShelf.Profiles
{
    public class CountryProfile : Profile
    {
        public CountryProfile()
        {
            CreateMap<CountryCreateModel, Country>(MemberList.Source);
            CreateMap<CountryUpdateModel, Country>(MemberList.Source).ReverseMap();
            
            CreateMap<Country, CountryViewModel>()
                .ForMember(x => x.Publishers,
                    act =>
                    {
                        act.MapFrom(x => x.Publishers.Select(x => x.Name));
                    });

        }
    }
}
