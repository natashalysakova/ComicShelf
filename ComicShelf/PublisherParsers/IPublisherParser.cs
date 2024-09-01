using AngleSharp;
using AngleSharp.Common;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using HtmlAgilityPack;
using Services.Services;

namespace ComicShelf.PublisherParsers
{
    public interface IPublisherParser
    {
        ParsedInfo Parse();
    }

    public abstract class BaseParser : IPublisherParser
    {
        protected BaseParser(string url)
        {
            this.url = url;
        }
        protected BaseParser(string url, PublishersService publishers) : this(url)
        {
            _publishers = publishers;
        }

        public ParsedInfo Parse()
        {
            var html = GetUrlHtml().Result;
            var parser = new HtmlParser();
            var document = parser.ParseDocument(html);

            var title = GetTitle(document);
            int volumeNumber = GetVolumeNumber(document);
            string series = GetSeries(document);
            string cover = GetCover(document);
            var parsed = new ParsedInfo(title, GetAuthors(document), volumeNumber, series, cover);

            return parsed;

        }

        protected abstract string GetTitle(IHtmlDocument document);
        protected abstract string GetSeries(IHtmlDocument document);
        protected abstract int GetVolumeNumber(IHtmlDocument document);
        protected abstract string GetAuthors(IHtmlDocument document);
        protected abstract string GetCover(IHtmlDocument document);
        protected abstract string PublisherName { get; }

        protected string url;
        private readonly PublishersService _publishers;



        protected async Task<string> GetUrlHtml()
        {
            HttpClient client = new HttpClient();
            string page = await client.GetStringAsync(url);
            return page;
        }
    }

    public class MalopusParser : BaseParser
    {
        public MalopusParser(string url) : base(url)
        {
        }

        protected override string PublisherName => "Mal'opus";

        protected override string GetAuthors(IHtmlDocument document)
        {
            var nodes = document.QuerySelectorAll(".rm-product-attr-list-item");

            foreach (var item in nodes)
            {
                var divs = item.ChildNodes.Where(x => x.NodeName == "DIV");
                var node = divs.FirstOrDefault(x => x.TextContent == "Автор");

                if (node is null)
                {
                    continue;
                }

                return divs.Last().TextContent;
            }

            return string.Empty;
        }

        protected override string GetCover(IHtmlDocument document)
        {
            var node = document.QuerySelector(".oct-gallery > img.img-fluid");
            var attribute = node.Attributes["src"];
            return attribute.Value;
        }

        protected override string GetSeries(IHtmlDocument document)
        {
            var nodes = document.QuerySelectorAll(".rm-product-center-info-item-title");
            var node = nodes.First(x => x.InnerHtml == "Серія:");
            return node.NextElementSibling.TextContent.Trim([' ', '\n']);
        }

        protected override string GetTitle(IHtmlDocument document)
        {
            var node = document.QuerySelector(".rm-product-title > h1");
            var title = node.InnerHtml.Substring(node.InnerHtml.LastIndexOf('.')+1).Trim();


            return title;
        }

        protected override int GetVolumeNumber(IHtmlDocument document)
        {
            var node = document.QuerySelector(".rm-product-title > h1");
            var title = node.InnerHtml.Substring(node.InnerHtml.LastIndexOf('.') + 1).Trim();

            var whitespace = title.IndexOf(' ');
            var nextWhiteSpace = title.IndexOf(' ', whitespace +1);
            string volume;
            if(nextWhiteSpace == -1)
            {
                volume = title.Substring(whitespace + 1).Trim();
            }
            else
            {
                volume = title.Substring(whitespace + 1, nextWhiteSpace - whitespace).Trim();
            }
            return int.Parse(volume);
        }
    }

    public class NashaIdeaParser : BaseParser
    {


        public NashaIdeaParser(string url) : base(url)
        {
        }

        protected override string PublisherName => "NashaIdea";

        protected override string GetAuthors(IHtmlDocument document)
        {
            return string.Empty;
        }

        protected override string GetCover(IHtmlDocument document)
        {
            var node = document.QuerySelectorAll(".woocommerce-product-gallery__image > a").First();
            var attribute = node.Attributes["href"];
            return attribute.Value;
        }

        protected override string GetSeries(IHtmlDocument document)
        {
            var tag = document.QuerySelector("h1.product_title");
            var title = tag.InnerHtml;

            var volIndex = title.IndexOf("Том");
            if (volIndex == -1)
            {
                return title.Trim();
            }
            else
            {
                var series = title.Substring(0, volIndex).Trim([' ', ',', '.']) ;
                return series;
            }
        }

        protected override string GetTitle(IHtmlDocument document)
        {
            var tag = document.QuerySelector("h1.product_title");
            var title = tag.InnerHtml;

            var volIndex = title.IndexOf("Том");
            if (volIndex == -1)
            {
                return title.Trim();
            }
            else
            {
                return title.Substring(volIndex).Trim();
            }
        }

        protected override int GetVolumeNumber(IHtmlDocument document)
        {
            var tag = document.QuerySelector("h1.product_title");
            var title = tag.InnerHtml;

            var volIndex = title.IndexOf("Том");
            var nextWord = title.IndexOf(" ", volIndex + 3);

            var nextWhitespace = title.IndexOf(" ", nextWord + 1);

            string volume;
            if (nextWhitespace == -1)
            {
                volume = title.Substring(nextWord).Trim();
            }
            else
            {
                volume = title.Substring(nextWord, nextWhitespace - nextWord).Trim();
            }


            return int.Parse(volume);
        }
    }

    public record ParsedInfo(string title, string authors, int volumeNumber, string series, string cover);
}
