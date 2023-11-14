using AutoMapper;
using Backend.Models;
using Services.ViewModels;

namespace Services.Profiles
{
    public class VolumeProfile : Profile
    {
        public VolumeProfile()
        {
            CreateMap<VolumeCreateModel, Volume>(MemberList.Source)
                .ForSourceMember(x => x.CoverFile, act => act.DoNotValidate())
                .ForSourceMember(x => x.SeriesName, act => act.DoNotValidate())
                .ForSourceMember(x => x.Authors, act => act.DoNotValidate())
                .ForSourceMember(x => x.NumberOfIssues, act => act.DoNotValidate())
                .ForSourceMember(x => x.NumberOfBonusIssues, act => act.DoNotValidate())
                .ForMember(x => x.OneShot, act => act.MapFrom(x => x.SingleVolume))
                .ForMember(x => x.Authors, act => act.Ignore());


            CreateMap<VolumeUpdateModel, Volume>(MemberList.Source)
                .ForSourceMember(x => x.CoverFile, act => { act.DoNotValidate(); })
                .ForSourceMember(x => x.Issues, act => act.DoNotValidate())
                .ForSourceMember(x => x.BonusIssues, act => act.DoNotValidate())

                .ForMember(x => x.Issues, act => act.MapFrom(x => x.Issues.Union(x.BonusIssues)));

            CreateMap<Volume, VolumeUpdateModel>()
                .ForMember(x => x.CoverFile, act => { act.Ignore(); })
                .ForMember(x => x.Issues, act => act.MapFrom(x => x.Issues.Where(y => y.GetType() == typeof(Issue))))
                .ForMember(x => x.BonusIssues, act => act.MapFrom(x => x.Issues.OfType<Bonus>()))
;


            CreateMap<Volume, IdNameView>().ForMember(x => x.Name, act => act.MapFrom(x => $"{x.Series.Name} {x.Title}"));
            CreateMap<Volume, VolumeViewModel>()
                .ForMember(x => x.SeriesPublisherCountryFlag,
                    act => act.MapFrom(x => x.Series.Publisher.Country.FlagPNG))
                .ForMember(x => x.SeriesName, act => act.MapFrom(x => x.Series.Name))
                .ForMember(x => x.Authors, act => act.MapFrom(x => x.Authors.Select(x => x.Name)))
                .ForMember(x => x.HasError, act => act.MapFrom(x => HasError(x)))
                .ForMember(x => x.SeriesPublisherUrl, act => act.MapFrom(x => x.Series.Publisher.Url))
                .ForMember(x => x.SeriesType, act => act.MapFrom(x => x.Series.Type.ToString()))
                .ForMember(x => x.SeriesTotalIssues, act => act.MapFrom(x => x.Series.TotalIssues))
                .ForMember(x => x.MalId, act => act.MapFrom(x => x.Series.MalId))
                .ForMember(x => x.Issues, act => act.MapFrom(x => x.Issues.Where(y => y.GetType() == typeof(Issue))))
                .ForMember(x => x.BonusIssues, act => act.MapFrom(x => x.Issues.OfType<Bonus>()))
                ;
        }

        private static bool HasError(Volume x)
        {
            return x.Expired();
        }
    }
}
