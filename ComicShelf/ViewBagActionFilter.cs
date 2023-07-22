using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NuGet.Configuration;
using System.Security.Policy;
using ComicShelf.Services;

namespace ComicShelf
{
    public class ViewBagActionFilter : ActionFilterAttribute
    {
        private readonly PublishersService publisherService;
        private readonly CountryService countryService;

        public ViewBagActionFilter(PublishersService publisherService, CountryService countryService)
        {
            this.publisherService = publisherService;
            this.countryService = countryService;

            //DI will inject what you need here

        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            //CoutryNotification
            //PublisherNotification
            //AuthorNotification
            //SeriesNotification
            //VolumeNotification

            // for razor pages
            if (context.Controller is PageModel)
            {
                var controller = context.Controller as PageModel;
                controller.ViewData.Add("PublisherNotification", publisherService.ShowNotification());

                //controller.ViewData.Add("Avatar", $"~/avatar/empty.png");
                // or
                //controller.ViewBag.Avatar = $"~/avatar/empty.png";

                //also you have access to the httpcontext & route in controller.HttpContext & controller.RouteData
            }

            //// for Razor Views
            //if (context.Controller is Controller)
            //{
            //    var controller = context.Controller as Controller;
            //    controller.ViewData.Add("Avatar", $"~/avatar/empty.png");
            //    // or
            //    controller.ViewBag.Avatar = $"~/avatar/empty.png";

            //    //also you have access to the httpcontext & route in controller.HttpContext & controller.RouteData
            //}

            base.OnResultExecuting(context);
        }
    }
}
