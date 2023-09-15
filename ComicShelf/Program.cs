using ComicShelf;
using ComicShelf.Models;
using ComicShelf.Services;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using System.Security.Policy;
using UnidecodeSharpFork;
using static System.Net.Mime.MediaTypeNames;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddDbContext<ComicShelfContext>();
        builder.Services.RegisterMyServices();

        builder.Services.AddLocalization(options => options.ResourcesPath = "Localization");

        builder.Services.AddMvc()
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            .AddDataAnnotationsLocalization();

        builder.Services.AddControllersWithViews(options => {
            options.Filters.Add<ViewBagActionFilter>();
        });

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
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred creating the DB.");
            }
        }

        

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapRazorPages();

        app.Run();
    }

    private static void RestoreImagesFromDB(ComicShelfContext context)
    {
        var volumes = context.Volumes.Include(x => x.Cover).Include(x => x.Series).ToList();
        foreach (var item in volumes)
        {
            if(item.Cover != null)
            {
                item.CoverUrl = FileUtility.RestoreCover(item.Series.Name, item.Number, item.Cover);
                context.SaveChanges();
            }
        }
    }
}


public static class FileUtility
{
    const string serverRoot = "..\\ComicShelf\\wwwroot";
    const string imageDir = "images";

    public static string RestoreCover(string seriesName, int volumeNumber, VolumeCover cover)
    {
        var escapedSeriesName = seriesName.Unidecode();

        var localDirectory = Path.Combine(serverRoot, imageDir, escapedSeriesName);
        var ext = cover.Extention;
        if (ext == string.Empty)
        {
            ext = ".jpg";
        }
        var filename = $"{escapedSeriesName} {volumeNumber}{ext}";
        var localPath = Path.Combine(localDirectory, filename);

        if (!Directory.Exists(localDirectory))
            Directory.CreateDirectory(localDirectory);

        File.WriteAllBytes(localPath, cover.Cover);

        return Path.Combine(imageDir, escapedSeriesName, filename);
    }

    internal static string DownloadFileFromWeb(string url, string seriesName, int volumeNumber, out byte[] image)
    {
        var ext = new FileInfo(url).Extension;
        var escapedSeriesName = seriesName.Unidecode();
        var destiantionFolder = $"{imageDir}\\{escapedSeriesName}";
        var filename = $"{escapedSeriesName} {volumeNumber}{ext}";
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

                    File.WriteAllBytes(localPath, imageBytes);

                }
            }

            return urlPath;
        }
        catch (Exception)
        {
            throw;
        }
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
}