using AutoMapper;
using Backend.Models;
using Services.ViewModels;

namespace ComicShelf.Profiles
{
    public class VolumeProfile : Profile
    {
        public VolumeProfile()
        {
            CreateMap<VolumeCreateModel, Volume>().ReverseMap();
            CreateMap<VolumeUpdateModel, Volume>().ReverseMap();
            CreateMap<VolumeViewModel, Volume>().ReverseMap();
        }
    }

    class AuthorsProfile : Profile
    {
        public AuthorsProfile()
        {
            CreateMap<AuthorCreateModel, Author>().ReverseMap();
            CreateMap<AuthorUpdateModel, Author>().ReverseMap();
            CreateMap<AuthorViewModel, Author>().ReverseMap();

        }
    }
    class PublisherProfile : Profile
    {
        public PublisherProfile()
        {
            CreateMap<PublisherCreateModel, Publisher>().ReverseMap();
            CreateMap<PublisherUpdateModel, Publisher>().ReverseMap();
            CreateMap<PublisherViewModel, Publisher>().ReverseMap();

        }
    }
    class CountryProfile : Profile
    {
        public CountryProfile()
        {
            CreateMap<CountryCreateModel, Country>().ReverseMap();
            CreateMap<CountryUpdateModel, Country>().ReverseMap();
            CreateMap<CountryViewModel, Country>().ReverseMap();

        }
    }

    class SeriesProfile : Profile
    {
        public SeriesProfile()
        {
            CreateMap<SeriesCreateModel, Series>().ReverseMap();
            CreateMap<SeriesUpdateModel, Series>().ReverseMap();
            CreateMap<SeriesViewModel, Series>().ReverseMap();

        }
    }
    class FiltersProfile : Profile
    {
        public FiltersProfile()
        {
            CreateMap<FilterCreateModel, Filter>().ReverseMap();
            CreateMap<FilterUpdateModel, Filter>().ReverseMap();
            CreateMap<FilterViewModel, Filter>().ReverseMap();

        }
    }
}
