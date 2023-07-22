using ComicShelf;
using ComicShelf.Models;
using ComicShelf.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Reflection;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddDbContext<ComicShelfContext>();
        builder.Services.RegisterMyServices();
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