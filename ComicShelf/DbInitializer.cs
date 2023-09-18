using ComicShelf.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Net.NetworkInformation;

internal class DbInitializer
{
    internal static void Initialize(ComicShelfContext context)
    {
        //context.Database.EnsureCreated();
        context.Database.Migrate();

        if (!context.Countries.Any(x => x.Name == "Unknown"))
        {
            context.Countries.Add(new Country() { FlagPNG = string.Empty, FlagSVG = string.Empty, Name = "Unknown", CountryCode = "Unknown" });
            context.SaveChanges();
        }

        if (context.Countries.Count() > 1)
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

            FileUtility.SaveFlagFromCDN(code, out string png, out string svg);

            context.Countries.Add(new Country() { FlagPNG = png, FlagSVG = svg, Name = country, CountryCode = code });
        }
        context.SaveChanges();

        var ukraine = context.Countries.Single(x => x.Name == "Ukraine");
        var us = context.Countries.Single(x => x.Name == "United States");
        var publisers = new Publisher[]
        {
            new Publisher(){ Name = "NashaIdea", Country = ukraine },
            new Publisher(){ Name = "Mal'opus", Country =ukraine },
            new Publisher(){ Name = "Mimir Media", Country = ukraine },
            new Publisher(){ Name = "Vovkulaka", Country = ukraine },
            new Publisher(){ Name = "Marvel", Country = us },
            new Publisher(){ Name = "DC Comics", Country = us },
            new Publisher(){ Name = "Dark Horse", Country = us },
            new Publisher(){ Name = "РМ", Country = ukraine }
        };

        foreach (var publisher in publisers)
        {
            context.Publishers.Add(publisher);
        }

        context.SaveChanges();
    }

    public static string CountryCode(string country)
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
    public static string IsoCountryCodeToFlagEmoji(string countryCode) => string.Concat(countryCode.ToUpper().Select(x => char.ConvertFromUtf32(x + 0x1F1A5)));
}