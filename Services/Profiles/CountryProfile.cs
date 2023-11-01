using AutoMapper;
using Backend.Models;
using Services.ViewModels;

namespace Services.Profiles
{
    public class CountryProfile : Profile
    {
        public CountryProfile()
        {
            CreateMap<CountryCreateModel, Country>(MemberList.Source);
            CreateMap<CountryUpdateModel, Country>(MemberList.Source).ReverseMap();

            CreateMap<Country, IdNameView>();
            CreateMap<Country, CountryViewModel>();

        }
    }
}
