using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Services;

namespace ComicShelf
{
    public class ViewBagActionFilter : ActionFilterAttribute
    {
        private readonly PublishersService publisherService;
        private readonly CountryService countryService;
        private readonly VolumeService volumeService;
        private readonly SeriesService seriesService;
        private readonly AuthorsService authorsService;

        public ViewBagActionFilter(PublishersService publisherService, CountryService countryService, VolumeService volumeService, SeriesService seriesService, AuthorsService authorsService)
        {
            this.publisherService = publisherService;
            this.countryService = countryService;
            this.volumeService = volumeService;
            this.seriesService = seriesService;
            this.authorsService = authorsService;

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
                controller.ViewData.Add("CountryNotification", countryService.ShowNotification());
                controller.ViewData.Add("VolumeNotification", volumeService.ShowNotification());
                controller.ViewData.Add("SeriesNotification", seriesService.ShowNotification());
                controller.ViewData.Add("AuthorsNotification", authorsService.ShowNotification());

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
