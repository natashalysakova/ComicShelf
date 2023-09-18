using ComicShelf;
using ComicShelf.Models;
using ComicShelf.Services;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Security.Policy;
using UnidecodeSharpFork;
using Microsoft.AspNetCore.DataProtection;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        if (builder.Environment.IsDevelopment())
        {
            builder.Configuration.AddUserSecrets<Program>();
        }
        else
        {
            builder.Configuration.AddJsonFile("connectionString.json");
        }



        // Add services to the container.
        builder.Services.AddRazorPages();

        //"server={ip};user id={db};password={password};database={dbName}"
        var connectionString = builder.Configuration["mariaDbConnectionString"];
        var version = ServerVersion.AutoDetect(connectionString);

        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddDbContext<ComicShelfContext>(
            options => options.UseMySql(connectionString, version)
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
        );
        }
        else
        {
            builder.Services.AddDbContext<ComicShelfContext>(
                options => options.UseMySql(connectionString, version)
            );
        }

        builder.Services.AddDbContext<ComicShelfContext>(
            options => options.UseMySql(connectionString, version)
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
        );

        builder.Services.RegisterMyServices();

        builder.Services.AddLocalization(options => options.ResourcesPath = "Localization");

        builder.Services.AddMvc()
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            .AddDataAnnotationsLocalization();

        builder.Services.AddControllersWithViews(options =>
        {
            options.Filters.Add<ViewBagActionFilter>();
        });

        //builder.Services.AddDataProtection().UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration()
        //        {
        //            EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
        //            ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
        //        });


        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        //using (var scope = app.Services.CreateScope())
        //{
        //    var services = scope.ServiceProvider;

        //    var context = services.GetRequiredService<ComicShelfContext>();
        //    context.Database.EnsureCreated();
        //    // DbInitializer.Initialize(context);
        //}

        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<ComicShelfContext>();
                DbInitializer.Initialize(context);
                RestoreImagesFromDB(context);
                FillFlags(context);
                FillColors(context);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred creating the DB.");
            }
        }


        app.UseStatusCodePages();
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapRazorPages();

        app.Run();
    }

    private static void FillFlags(ComicShelfContext context)
    {
        foreach (var item in context.Countries)
        {
            if (String.IsNullOrEmpty(item.FlagPNG) || String.IsNullOrEmpty(item.FlagSVG))
            {
                item.CountryCode = item.CountryCode.ToLower();
                FileUtility.SaveFlagFromCDN(item.CountryCode, out string png, out string svg);
                item.FlagPNG = png;
                item.FlagSVG = svg;
            }
        }

        context.SaveChanges();
    }

    private static void RestoreImagesFromDB(ComicShelfContext context)
    {
        var volumes = context.Volumes.Include(x => x.Series).ToList();
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
            context.SaveChanges();
        }

    }

    private static void FillColors(ComicShelfContext context)
    {
        foreach (var item in context.Series)
        {
            if (string.IsNullOrEmpty(item.Color) || string.IsNullOrEmpty(item.ComplimentColor))
            {
                var color = ColorUtility.GetRandomColor(minSaturation: 60, minValue: 60);
                var complementary = ColorUtility.GetOppositeColor(color);

                item.Color = ColorUtility.HexConverter(color);
                item.ComplimentColor = ColorUtility.HexConverter(complementary);
            }
        }

        context.SaveChanges();


    }
}


public static class FileUtility
{
#if DEBUG
    const string serverRoot = "..\\ComicShelf\\wwwroot";
#else
    const string serverRoot = "/volume1/web/publish/wwwroot";
#endif

    const string imageDir = "images";

    //public static string RestoreCover(string seriesName, int volumeNumber, VolumeCover cover)
    //{
    //    var escapedSeriesName = seriesName.Unidecode().Replace(Path.GetInvalidFileNameChars(), string.Empty);

    //    var localDirectory = Path.Combine(serverRoot, imageDir, "Series", escapedSeriesName);
    //    var ext = cover.Extention;
    //    if (ext == string.Empty)
    //    {
    //        ext = ".jpg";
    //    }
    //    var filename = $"{escapedSeriesName} {volumeNumber}{ext}";
    //    var localPath = Path.Combine(localDirectory, filename);

    //    if (!Directory.Exists(localDirectory))
    //        Directory.CreateDirectory(localDirectory);

    //    File.WriteAllBytes(localPath, cover.Cover);

    //    return Path.Combine(imageDir, "Series", escapedSeriesName, filename);
    //}

    internal static string DownloadFileFromWeb(string url, string seriesName, int volumeNumber, out byte[] image, out string extention)
    {
        extention = new FileInfo(url).Extension;
        var escapedSeriesName = seriesName.Unidecode().Replace(Path.GetInvalidFileNameChars(), string.Empty);
        var destiantionFolder = $"{imageDir}\\{escapedSeriesName}";
        var filename = $"{escapedSeriesName} {volumeNumber}{extention}";
        var urlPath = Path.Combine(destiantionFolder, filename);

        try
        {
            using (var client = new HttpClient())
            {
                using (var response = client.GetAsync(url))
                {
                    byte[] imageBytes =
                        response.Result.Content.ReadAsByteArrayAsync().Result;
                    image = imageBytes;

                    var localDirectory = Path.Combine(serverRoot, destiantionFolder);
                    var localPath = Path.Combine(localDirectory, filename);

                    if (!Directory.Exists(localDirectory))
                        Directory.CreateDirectory(localDirectory);

                    System.IO.File.WriteAllBytes(localPath, imageBytes);

                }
            }

            return urlPath;
        }
        catch (Exception)
        {
            throw;
        }
    }

    internal static string SaveOnServer(IFormFile coverFile, string seriesName, int volumeNumber, out string extention)
    {
        extention = new FileInfo(coverFile.FileName).Extension;
        var escapedSeriesName = seriesName.Unidecode().Replace(Path.GetInvalidFileNameChars(), string.Empty);
        var destiantionFolder = $"{imageDir}\\Series\\{escapedSeriesName}";
        var filename = $"{escapedSeriesName} {volumeNumber} {coverFile.GetHashCode()}{extention}";
        var urlPath = Path.Combine(destiantionFolder, filename);

        try
        {
            var localDirectory = Path.Combine(serverRoot, destiantionFolder);
            var localPath = Path.Combine(localDirectory, filename);

            if (!Directory.Exists(localDirectory))
                Directory.CreateDirectory(localDirectory);

            using (var fileStream = new FileStream(localPath, FileMode.Create))
            {
                coverFile.CopyTo(fileStream);
            }
            return urlPath;
        }
        catch (Exception)
        {
            throw;
        }
    }

    internal static void SaveFlagFromCDN(string countryCode, out string png, out string svg)
    {
        var urls = new List<string> {
            $"https://flagcdn.com/{countryCode}.svg",
            $"https://flagcdn.com/40x30/{countryCode}.png" };

        var destiantionFolder = $"{imageDir}\\countries";

        foreach (var url in urls)
        {
            var extention = Path.GetExtension(url);
            var filename = $"{countryCode}{extention}";

            using (var client = new HttpClient())
            {
                using (var response = client.GetAsync(url))
                {
                    byte[] imageBytes =
                        response.Result.Content.ReadAsByteArrayAsync().Result;

                    var localDirectory = Path.Combine(serverRoot, destiantionFolder);
                    var localPath = Path.Combine(localDirectory, filename);

                    if (!Directory.Exists(localDirectory))
                        Directory.CreateDirectory(localDirectory);

                    File.WriteAllBytes(localPath, imageBytes);
                }
            }
        }

        png = Path.Combine(destiantionFolder, $"{countryCode}.png");
        svg = Path.Combine(destiantionFolder, $"{countryCode}.svg");

    }

    internal static string FindUrl(string coverUrl)
    {
        var localPath = Path.Combine(serverRoot, coverUrl);
        if (File.Exists(localPath))
        {
            return coverUrl;
        }
        else
        {
            var dir = Path.GetDirectoryName(localPath);
            var patten = $"{Path.GetFileNameWithoutExtension(localPath)} *{Path.GetExtension(localPath)}";
            var files = Directory.GetFiles(dir, patten);

            if (files.Length == 1)
            {
                return Path.Combine(Path.GetDirectoryName(coverUrl), Path.GetFileName(files[0]));
            }

        }
        return coverUrl;
    }
}

public static class ColorUtility
{
    private static Random random;

    static Random Random
    {
        get
        {
            if (random == null)
                random = new Random();
            return random;
        }
    }

    public static Color GetRandomColor()
    {
        return Color.FromArgb(Random.Next(0, 255), Random.Next(0, 255), Random.Next(0, 255));
    }
    public static Color GetRandomColor(
        int minHue = 0, int maxHue = 360,
        int minSaturation = 0, int maxSaturation = 100,
        int minValue = 0, int maxValue = 100)
    {
        var hue = Random.Next(minHue, maxHue);
        var saturation = Random.NextDoublePercent(minSaturation, maxSaturation);
        var value = Random.NextDoublePercent(minValue, maxValue);

        return ColorFromHSV(hue, saturation, value);
    }

    public static Color GetOppositeColor(Color original)
    {
        ColorToHSV(original, out double hue, out double saturation, out double value);

        hue = (hue + 180) % 360;

        return ColorFromHSV(hue, saturation, value);
    }

    public static String HexConverter(Color c)
    {
        return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
    }

    private static void ColorToHSV(Color color, out double hue, out double saturation, out double value)
    {
        int max = Math.Max(color.R, Math.Max(color.G, color.B));
        int min = Math.Min(color.R, Math.Min(color.G, color.B));

        hue = color.GetHue();
        saturation = (max == 0) ? 0 : 1d - (1d * min / max);
        value = max / 255d;
    }

    private static Color ColorFromHSV(double hue, double saturation, double value)
    {
        int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
        double f = hue / 60 - Math.Floor(hue / 60);

        value = value * 255;
        int v = Convert.ToInt32(value);
        int p = Convert.ToInt32(value * (1 - saturation));
        int q = Convert.ToInt32(value * (1 - f * saturation));
        int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

        if (hi == 0)
            return Color.FromArgb(255, v, t, p);
        else if (hi == 1)
            return Color.FromArgb(255, q, v, p);
        else if (hi == 2)
            return Color.FromArgb(255, p, v, t);
        else if (hi == 3)
            return Color.FromArgb(255, p, q, v);
        else if (hi == 4)
            return Color.FromArgb(255, t, p, v);
        else
            return Color.FromArgb(255, v, p, q);
    }

    internal static Color ChangeSaturation(Color color, int percent)
    {
        ColorToHSV(color, out double hue, out double saturation, out double value);

        saturation += (percent / 100.0);
        if (saturation > 1)
            saturation = 1;
        if (saturation < 0)
            saturation = 0;

        return ColorFromHSV(hue, saturation, value);
    }
    internal static Color ChangeValue(Color color, int percent)
    {
        ColorToHSV(color, out double hue, out double saturation, out double value);

        value += (percent / 100.0);
        if (value > 1)
            value = 1;
        if (value < 0)
            value = 0;

        return ColorFromHSV(hue, saturation, value);
    }
}

public static class Extentions
{
    public static IServiceCollection RegisterMyServices(this IServiceCollection services)
    {
        var type = typeof(IService);
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => type.IsAssignableFrom(p) && !p.IsAbstract);

        foreach (var item in types)
        {
            services.AddScoped(item);
        }

        return services;
    }

    public static double NextDoublePercent(this Random random, int min = 0, int max = 100)
    {
        if (min > max || min < 0)
            throw new ArgumentException("Min should be between 0 and max");

        if (max > 100 || max < 0)
            throw new ArgumentException("Max should be between 0 and 100");


        return random.Next(min, max) / 100.0;
    }

    public static string Replace(this string s, char[] separators, string newVal)
    {
        string[] temp;

        temp = s.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        return String.Join(newVal, temp);
    }
}