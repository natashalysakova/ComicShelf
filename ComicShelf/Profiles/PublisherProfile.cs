using AutoMapper;
using Backend.Models;
using Services.ViewModels;

namespace ComicShelf.Profiles
{
    public class PublisherProfile : Profile
    {
        public PublisherProfile()
        {
            CreateMap<PublisherCreateModel, Publisher>(MemberList.Source);
            CreateMap<PublisherUpdateModel, Publisher>(MemberList.Source).ReverseMap();
            CreateMap<Publisher, PublisherViewModel>()
                .ForMember(x=>x.Series, act =>
                {
                    act.MapFrom(x=>x.Series.Select(x=>x.Name).Distinct());
                })
                ;

        }
    }
}
