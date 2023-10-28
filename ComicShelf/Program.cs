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
using ComicShelf.Services;
using ComicShelf.Utilities;
using ComicShelf.Localization;
using Microsoft.Extensions.Localization;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();


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

        RegisterMyServices(builder.Services);

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


        builder.Services.AddSingleton(typeof(LocalizationService));
        builder.Services.AddSingleton(typeof(EnumUtilities));

        builder.Services.AddSession(option =>
        {
            option.Cookie.Name = "VolumeFilters";
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
                var dbInitializer = new DbInitializer(context);

                dbInitializer.Initialize();
                //dbInitializer.RestoreImagesFromDB();
                dbInitializer.FillFlags();
                dbInitializer.FillColors();
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred creating the DB.");
                throw;
            }
        }

        app.UseStatusCodePages();
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();
        app.UseSession();
        app.MapRazorPages();

        app.Run();
    }

    public static IServiceCollection RegisterMyServices(IServiceCollection services)
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
