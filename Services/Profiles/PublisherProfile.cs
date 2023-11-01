using AutoMapper;
using Backend.Models;
using Services.ViewModels;

namespace Services.Profiles
{
    public class PublisherProfile : Profile
    {
        public PublisherProfile()
        {
            CreateMap<PublisherCreateModel, Publisher>(MemberList.Source);
            CreateMap<PublisherUpdateModel, Publisher>(MemberList.Source).ReverseMap();

            CreateMap<Publisher, IdNameView>();
            CreateMap<Publisher, PublisherViewModel>();

        }
    }
}
