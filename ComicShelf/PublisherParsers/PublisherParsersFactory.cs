
namespace ComicShelf.PublisherParsers
{
    public class PublisherParsersFactory
    {
        internal IPublisherParser? CreateParser(string url)
        {
            if (url.StartsWith("https://nashaidea.com/"))
            {
                return new NashaIdeaParser(url);
            }
            if (url.StartsWith("https://malopus.com.ua/"))
            {
                return new NashaIdeaParser(url);
            }

            return default;
        }
    }
}
