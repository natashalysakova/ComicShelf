using AutoMapper;
using AutoMapper.Internal;
using Backend.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.VisualBasic;
using Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Profiles
{
    internal class HistotyProfile : Profile
    {
        string dateFormat = "dd-MM-yyyy";

        public HistotyProfile()
        {
            var converter = new DateTimeToStringConverter(dateFormat);

            CreateMap<History, HistoryViewModel>()
                .ForMember(x => x.WishlistedDate, act => act.MapFrom(y => converter.Convert(y.WishlistedDate)))
                .ForMember(x => x.ReleaseDate, act => act.MapFrom(y => converter.Convert(y.ReleaseDate)))
                .ForMember(x => x.PurchaseDate, act => act.MapFrom(y => converter.Convert(y.PurchaseDate)))
                .ForMember(x => x.ReadDate, act => act.MapFrom(y => converter.Convert(y.ReadDate)))
                .ForMember(x => x.AnnouncedDate, act => act.MapFrom(y => converter.Convert(y.AnnouncedDate)))
                .ForMember(x => x.GivedAwayDate, act => act.MapFrom(y => converter.Convert(y.GivedAwayDate)))
                .ForMember(x => x.PreorderedDate, act => act.MapFrom(y => converter.Convert(y.PreorderedDate)))
                    ;

        }

    }

    public class DateTimeToStringConverter 
    {
        private readonly string _format;

        public DateTimeToStringConverter(string format)
        {
            _format = format;
        }

        public string Convert(DateTime? sourceMember)
        {
            if (sourceMember == null)
                return string.Empty;

            return sourceMember.Value.ToString(_format);
        }
    }
}
