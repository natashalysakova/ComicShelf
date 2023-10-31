using AutoMapper;
using Backend.Models;
using Services.ViewModels;

namespace ComicShelf.Profiles
{
    public class SeriesProfile : Profile
    {
        public SeriesProfile()
        {
            CreateMap<SeriesCreateModel, Series>(MemberList.Source);

            CreateMap<SeriesUpdateModel, Series>()
                .ForMember(x => x.ComplimentColor, act => { act.Ignore(); })
                .ForMember(x => x.Publisher, act => { act.Ignore(); })
                .ForMember(x => x.Volumes, act => { act.Ignore(); });


            CreateMap<Series, SeriesUpdateModel>()
                .ForMember(x => x.VolumeCount, act =>
                {
                    act.MapFrom(x => x.Volumes.Count);
                });

            CreateMap<Series, SeriesViewModel>()
                .ForMember(x => x.VolumesCount, act =>
                {
                    act.MapFrom(source => source.Volumes.Count);
                });

        }
    }
}
