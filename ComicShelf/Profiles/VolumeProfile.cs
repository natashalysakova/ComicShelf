using AutoMapper;
using Backend.Models;
using Services.ViewModels;

namespace ComicShelf.Profiles
{
    public class VolumeProfile : Profile
    {
        public VolumeProfile()
        {
            CreateMap<VolumeCreateModel, Volume>(MemberList.Source)
                .ForSourceMember(x => x.CoverFile, act => act.DoNotValidate())
                .ForSourceMember(x => x.SeriesName, act => act.DoNotValidate())
                .ForSourceMember(x => x.Authors, act => act.DoNotValidate())
                .ForMember(x => x.OneShot, act => act.MapFrom(x => x.SingleVolume))
                .ForMember(x => x.Authors, act => act.Ignore());


            CreateMap<VolumeUpdateModel, Volume>(MemberList.Source)
                .ForSourceMember(x => x.CoverFile, act => { act.DoNotValidate(); });

            CreateMap<Volume, VolumeUpdateModel>()
                .ForMember(x => x.CoverFile, act => { act.Ignore(); });


            CreateMap<Volume, VolumeViewModel>()
                .ForMember(x => x.SeriesPublisherCountryFlag, act =>
                {
                    act.MapFrom(x => x.Series.Publisher.Country.FlagPNG);
                })
                .ForMember(x => x.SeriesName, act =>
                {
                    act.MapFrom(x => x.Series.Name);
                })
                .ForMember(x=>x.Authors, act=> act.MapFrom(x=>x.Authors.Select(x=>x.Name)))
                ;
        }
    }
}
