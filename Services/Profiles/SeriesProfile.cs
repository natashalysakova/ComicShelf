using AutoMapper;
using Backend.Models;
using Services.ViewModels;

namespace Services.Profiles
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
                .ForMember(x => x.VolumeCount, act => act.MapFrom(x => x.Volumes.Count))
                .ForMember(x => x.HasError, act => act.MapFrom(y => HasError(y)));

            CreateMap<Series, IdNameView>();
            CreateMap<Series, SeriesViewModel>()
                .ForMember(x => x.VolumesCount, act => act.MapFrom(source => source.Volumes.Count));


        }

        private static bool HasError(Series item)
        {
            return item.PublisherId <= 1 || (!item.Ongoing && item.TotalVolumes == 0);
        }
    }
}
