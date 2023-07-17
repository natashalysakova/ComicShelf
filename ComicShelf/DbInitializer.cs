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

        var countries = new Country[]
        {
            new Country(){ Name = "Ukraine", Flag = GetFlag("Ukraine")},
            new Country(){ Name = "United States", Flag = GetFlag("United States")},
            new Country(){Name = "Japan", Flag = GetFlag("Japan") }
        };

        foreach (var country in countries)
        {
            context.Countries.Add(country);
        }

        context.SaveChanges();

        var publisers = new Publisher[]
        {
            new Publisher(){ Name = "NashaIdea", Country = countries[0] },
            new Publisher(){ Name = "Mal'opus", Country =countries[0] },
            new Publisher(){ Name = "Mimir Media", Country = countries[0] },
            new Publisher(){ Name = "Vovkulaka", Country = countries[0] },
            new Publisher(){ Name = "Marvel", Country = countries[1] },
            new Publisher(){ Name = "DC Comics", Country = countries[1] },
            new Publisher(){ Name = "Dark Horse", Country = countries[1] }
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