using ComicShelf;
using ComicShelf.Models;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Reflection;
using System.Security.Policy;
using Microsoft.AspNetCore.DataProtection;
using System.Globalization;

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

        builder.Services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new[] { "uk-UA", "en" };
            options.SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);
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

        app.UseRequestLocalization(new RequestLocalizationOptions
        {
            ApplyCurrentCultureToResponseHeaders = true
        });
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
