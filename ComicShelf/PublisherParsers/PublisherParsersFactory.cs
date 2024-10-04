
using Services.Services;
using System.Reflection;

namespace ComicShelf.PublisherParsers
{
    public class PublisherParsersFactory
    {
        private readonly IEnumerable<IPublisherParser> _parsers;

        public PublisherParsersFactory()
        {
            _parsers = [
                new NashaIdeaParser(),
                new MalopusParser(),
                new AmazonParser(),
                new KoboParser()
                ];
        }

    public IPublisherParser? CreateParser(string url)
    {
        foreach (var parser in _parsers)
        {
            if (url.StartsWith(parser.SiteUrl))
            {
                parser.SetUrl(url);
                return parser;
            }
        }

        return default;
    }
}
}
