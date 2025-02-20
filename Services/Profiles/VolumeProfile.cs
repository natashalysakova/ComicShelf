﻿using AutoMapper;
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
                .ForSourceMember(x => x.VolumeType, act => act.DoNotValidate())
                .ForSourceMember(x => x.CoverToDownload, act => act.DoNotValidate())
                .ForSourceMember(x => x.PublisherName, act => act.DoNotValidate())
                .ForSourceMember(x => x.TotalVolumes, act => act.DoNotValidate())
                .ForSourceMember(x => x.SeriesStatus, act => act.DoNotValidate())
                .ForSourceMember(x => x.SeriesOriginalName, act => act.DoNotValidate())
                .ForMember(x => x.OneShot, act => act.MapFrom(x => x.VolumeType == VolumeItemType.OneShot))
                .ForMember(x => x.SingleIssue, act => act.MapFrom(x => x.VolumeType == VolumeItemType.Issue))
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
                .ForMember(x => x.IssuesRange, act => act.MapFrom(x => FormatRanges(x.Issues.Where(x => x.GetType() == typeof(Issue)).Select(y => y.Number).OrderBy(x => x).ToArray())));
        }


        private static bool HasError(Volume x)
        {
            return x.Expired();
        }

        private static string FormatRanges(int[] arr)
        {
            if (arr.Length == 0)
                return string.Empty;

            List<string> result = new List<string>();
            int start = arr[0];
            int end = arr[0];

            foreach (int num in arr.Skip(1))
            {
                if (num == end + 1)
                {
                    end = num;
                }
                else
                {
                    result.Add(start == end ? start.ToString() : $"{start}-{end}");
                    start = end = num;
                }
            }

            result.Add(start == end ? start.ToString() : $"{start}-{end}");

            return string.Join(", ", result);
        }
    }
}
