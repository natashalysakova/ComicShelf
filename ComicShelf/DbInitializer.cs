﻿using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Services.Services;
using Services.Services.Enums;
using System.Diagnostics.Metrics;
using System.Globalization;

internal class DbInitializer
{
    private readonly ComicShelfContext _context;

    internal DbInitializer(ComicShelfContext context)
    {
        _context = context;
    }
    private const string unknown = "Unknown";
    internal void Initialize()
    {
        //context.Database.EnsureCreated();
        _context.Database.Migrate();

        if (!_context.Countries.Any(x => x.Name == unknown))
        {
            _context.Countries.Add(new Country() { FlagPNG = string.Empty, FlagSVG = string.Empty, Name = unknown, CountryCode = unknown });
            _context.SaveChanges();
        }

        var unknownCountry = _context.Countries.Single(x => x.Name == unknown);
        if (!_context.Publishers.Any(x => x.Name == unknown))
        {
            _context.Publishers.Add(new Publisher() { Country = unknownCountry, Name = unknown, Url = string.Empty });
            _context.SaveChanges();
        }


        var standartFilters = new Filter[]
            {
                new Filter()
                {
                    Name = "AllAvailable",
                    DisplayOrder = 0,
                    Group = FilterService.STANDART,
                    Json = JsonConvert.SerializeObject(new BookshelfParams(){ filter = PurchaseFilterEnum.Available, sort = SortEnum.ByPurchaseDate})
                },
                new Filter()
                {
                    Name = "Finished",
                    DisplayOrder = 1,
                    Group = FilterService.STANDART,
                    Json = JsonConvert.SerializeObject(new BookshelfParams(){ filter = PurchaseFilterEnum.Available, sort = SortEnum.ByPurchaseDate, reading = ReadingEnum.Completed })
                },
                new Filter()
                {
                    Name = "NewSeries",
                    DisplayOrder = 2,
                    Group = FilterService.STANDART,
                    Json = JsonConvert.SerializeObject(new BookshelfParams(){ filter = PurchaseFilterEnum.Available, sort = SortEnum.ByPurchaseDate, reading = ReadingEnum.NewSeries})
                },
            };

        if (_context.Filters.Count(x => x.Group == FilterService.STANDART) != standartFilters.Length)
        {
            foreach (var item in standartFilters)
            {
                if (_context.Filters.SingleOrDefault(x => x.Name == item.Name) == default)
                {
                    _context.Filters.Add(item);
                }
            }

            _context.SaveChanges();
        }

        foreach (var item in _context.Filters)
        {
            if (string.IsNullOrEmpty(item.Group))
            {
                item.Group = FilterService.CUSTOM;
            }
        }
        _context.SaveChanges();

        if(_context.Anime.Count() == 0)
        {
            var anime = new Anime()
            {
                Title = "test",
                OriginalTitle = "tesutu"
            };

            var season = new Season()
            {
                Anime = anime,
                HierarchyOrder = 1,
                ReleaseDate = DateTime.Today,
                Title = "season 1"
            };

            var movie = new Movie()
            {
                Anime = anime,
                HierarchyOrder = 1,
                ReleaseDate = DateTime.Today,
                Title = "movie"
            };

            var special = new Special()
            {
                Anime = anime,
                HierarchyOrder = 1,
                ReleaseDate = DateTime.Today,
                Title = "special"
            };

            _context.Anime.Add(anime); ;

            _context.Items.AddRange(season, movie, special);
            _context.SaveChanges();
        }



        if (_context.Countries.Count() > 1)
        {
            return;
        }



        CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
        var regions = cultures.Select(x => new RegionInfo(x.Name));
        var results = regions.Select(x => x.EnglishName).Distinct().OrderBy(x => x).ToList();

        foreach (var country in results)
        {
            var code = CountryCode(country);

            if (code.Length != 2)
                continue;

            Console.WriteLine(code);
            try
            {
                FileUtility.SaveFlagFromCDN(code, out string png, out string svg);

                _context.Countries.Add(new Country() { FlagPNG = png, FlagSVG = svg, Name = country, CountryCode = code });

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return;
            }
        }
        _context.SaveChanges();

        var ukraine = _context.Countries.Single(x => x.Name == "Ukraine");
        var us = _context.Countries.Single(x => x.Name == "United States");
        var publisers = new Publisher[]
        {
            new Publisher(){ Name = "NashaIdea", Country = ukraine, Url = string.Empty },
            new Publisher(){ Name = "Mal'opus", Country =ukraine, Url = string.Empty },
            new Publisher(){ Name = "Mimir Media", Country = ukraine, Url = string.Empty },
            new Publisher(){ Name = "Vovkulaka", Country = ukraine, Url = string.Empty },
            new Publisher(){ Name = "Marvel", Country = us, Url = string.Empty },
            new Publisher(){ Name = "DC Comics", Country = us, Url = string.Empty },
            new Publisher(){ Name = "Dark Horse", Country = us, Url = string.Empty },
            new Publisher(){ Name = "РМ", Country = ukraine, Url = string.Empty }
        };

        foreach (var publisher in publisers)
        {
            _context.Publishers.Add(publisher);
        }

        _context.SaveChanges();
    }

    internal string CountryCode(string country)
    {
        var regions = CultureInfo.GetCultures(CultureTypes.SpecificCultures).Select(x => new RegionInfo(x.Name)).ToList();
        var englishRegion = regions.FirstOrDefault(region => region.EnglishName.Contains(country));
        string countryCode = string.Empty;
        if (englishRegion != null)
        {
            countryCode = englishRegion.TwoLetterISORegionName;
        }

        return countryCode.ToLower();
    }
    public string IsoCountryCodeToFlagEmoji(string countryCode) => string.Concat(countryCode.ToUpper().Select(x => char.ConvertFromUtf32(x + 0x1F1A5)));

    internal void FillFlags()
    {
        try
        {
            foreach (var item in _context.Countries)
            {
                if (String.IsNullOrEmpty(item.FlagPNG) || String.IsNullOrEmpty(item.FlagSVG))
                {
                    item.CountryCode = item.CountryCode.ToLower();
                    FileUtility.SaveFlagFromCDN(item.CountryCode, out string png, out string svg);
                    item.FlagPNG = png;
                    item.FlagSVG = svg;
                }
            }

            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return;
        }
    }

    internal void RestoreImagesFromDB()
    {
        var volumes = _context.Volumes.Include(x => x.Series).ToList();
        foreach (var item in volumes)
        {
            //if (item.Cover.Cover != null)
            //{
            //    item.CoverUrl = FileUtility.RestoreCover(item.Series.Name, item.Number, item.Cover);
            //}


            item.CoverUrl = FileUtility.FindUrl(item.CoverUrl);


            if (item.CreationDate == default)
            {
                item.CreationDate = DateTime.Now;
            }
            _context.SaveChanges();
        }

    }


    internal void FillColors()
    {
        foreach (var item in _context.Series)
        {
            if (string.IsNullOrEmpty(item.Color) || string.IsNullOrEmpty(item.ComplimentColor))
            {
                var color = ColorUtility.GetRandomColor(minSaturation: 60, minValue: 60);
                var complementary = ColorUtility.GetOppositeColor(color);

                item.Color = ColorUtility.HexConverter(color);
                item.ComplimentColor = ColorUtility.HexConverter(complementary);
            }
        }

        _context.SaveChanges();


    }

    internal void MigrateHistory()
    {
        if (_context.History.Any()) 
            return;

        foreach (var item in _context.Volumes)
        {
            item.History = new History()
            {
                PreorderedDate = item.PreorderDate,
                PurchaseDate = item.PurchaseDate,
                ReleaseDate = item.ReleaseDate
            };
        }

        _context.SaveChanges();
    }
}