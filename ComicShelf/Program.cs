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
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Builder;
using MySqlConnector;

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

        ServerVersion version = default;

        do
        {
            try
            {
                Console.WriteLine("connecting to " + connectionString);
                version = ServerVersion.AutoDetect(connectionString);
                Console.WriteLine("Success");
            }
            catch (MySqlException ex)
            {
                if (ex.Message.Contains("Unable to connect to any of the specified MySQL hosts"))
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Trying in 5 seconds");
                    Thread.Sleep(5000);
                }
                else
                {
                    throw ex;
                }
            }           
        }
        while (version is null);
        

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

        var supportedCultures = new[] {
            new CultureInfo("uk-UA"),
            new CultureInfo("en")
        };
        builder.Services.AddLocalization(options => options.ResourcesPath = "Localization");
        builder.Services.Configure<RequestLocalizationOptions>(options =>
        {
            
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
            options.DefaultRequestCulture = new RequestCulture("uk-UA", "uk-UA");
        });


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

        var locService = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
        app.UseRequestLocalization(locService.Value);


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
