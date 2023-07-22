using ComicShelf.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using System.Globalization;

internal class DbInitializer
{
    internal static void Initialize(ComicShelfContext context)
    {
        //context.Database.EnsureCreated();
        context.Database.Migrate();

        if (context.Countries.Any())
        {
            return;
        }


        CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
        var regions = cultures.Select(x => new RegionInfo(x.Name));
        var results = regions.Select(x => x.EnglishName).Distinct().Order().ToList();

        var countries = results.Select(x => new Country() { Flag = GetFlag(x), Name = x });
        foreach (var country in countries)
        {
            context.Countries.Add(country);
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
            new Publisher(){ Name = "Dark Horse", Country = us }
        };

        foreach(var publisher in publisers)
        {
            context.Publishers.Add(publisher);
        }

        context.SaveChanges();
    }

    public static string GetFlag(string country)
    {
        var regions = CultureInfo.GetCultures(CultureTypes.SpecificCultures).Select(x => new RegionInfo(x.Name)).ToList();
        var englishRegion = regions.FirstOrDefault(region => region.EnglishName.Contains(country));
        if (englishRegion == null) return "🏳";
        var countryAbbrev = englishRegion.TwoLetterISORegionName;
        return IsoCountryCodeToFlagEmoji(countryAbbrev);
    }
    public static string IsoCountryCodeToFlagEmoji(string countryCode) => string.Concat(countryCode.ToUpper().Select(x => char.ConvertFromUtf32(x + 0x1F1A5)));
}