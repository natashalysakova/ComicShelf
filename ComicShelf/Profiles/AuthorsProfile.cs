using AutoMapper;
using Backend.Models;
using Services.ViewModels;

namespace ComicShelf.Profiles
{
    public class AuthorsProfile : Profile
    {
        public AuthorsProfile()
        {
            CreateMap<AuthorCreateModel, Author>(MemberList.Source);
            CreateMap<AuthorUpdateModel, Author>(MemberList.Source).ReverseMap();

            CreateMap<Author, AuthorViewModel>().ForMember(x => x.SeriesNames, act =>
            {
                act.MapFrom(source => source.Volumes.Select(x => x.Series.Name).ToList().Distinct());
            });

        }
    }
}
