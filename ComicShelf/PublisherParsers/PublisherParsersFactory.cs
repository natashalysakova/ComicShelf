
using Services.Services;

namespace ComicShelf.PublisherParsers
{
    public class PublisherParsersFactory
    {
        private readonly IConfiguration _configuration;
        private readonly PublishersService _publishersService;

        public PublisherParsersFactory(IConfiguration configuration, PublishersService publishersService)
        {
            _configuration = configuration;
            _publishersService = publishersService;
        }

        public IPublisherParser? CreateParser(string url)
        {
            if (url.StartsWith("https://nashaidea.com/product/"))
            {
                return new NashaIdeaParser(url, _configuration);
            }
            if (url.StartsWith("https://malopus.com.ua/"))
            {
                return new MalopusParser(url, _configuration);
            }
            if (url.StartsWith("https://www.kobo.com/"))
            {
                return new KoboParser(url, _configuration);
            }

            return default;
        }
    }
}
