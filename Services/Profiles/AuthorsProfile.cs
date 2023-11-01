using AutoMapper;
using Backend.Models;
using Services.ViewModels;

namespace Services.Profiles
{
    public class AuthorsProfile : Profile
    {
        public AuthorsProfile()
        {
            CreateMap<AuthorCreateModel, Author>(MemberList.Source);
            CreateMap<AuthorUpdateModel, Author>(MemberList.Source)
                .ForSourceMember(x => x.HasError, act => act.DoNotValidate())
                .ForSourceMember(x => x.Series, act => act.DoNotValidate())
                .ReverseMap()
                .ForMember(x => x.HasError, act => act.MapFrom(y => y.Roles == Backend.Models.Enums.Roles.None))
                .ForMember(x => x.Series, act => act.MapFrom(y => y.Volumes.Select(x => x.Series)));


            CreateMap<Author, IdNameView>();
            CreateMap<Author, AuthorViewModel>()
;

        }
    }
}
