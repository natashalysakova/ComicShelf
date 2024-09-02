using AngleSharp;
using AngleSharp.Common;
using AngleSharp.Dom;
using AngleSharp.Html;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Backend.Models.Enums;
using HtmlAgilityPack;
using Services.Services;
using System.Text;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace ComicShelf.PublisherParsers
{
    public interface IPublisherParser
    {
        Task<ParsedInfo> Parse();
    }

    public abstract class BaseParser : IPublisherParser
    {
        protected BaseParser(string url, IConfiguration configuration)
        {
            this.url = url;
            _configuration = configuration;
        }


        protected BaseParser(string url, IConfiguration configuration, PublishersService publishers) : this(url, configuration)
        {
            _publishers = publishers;
        }

        public async Task<ParsedInfo> Parse()
        {
            //var config = new Configuration().WithDefaultLoader();
            //var document = await BrowsingContext.New(config).OpenAsync(url);

            var html = GetUrlHtml().Result;
            var parser = new HtmlParser();
            var document = parser.ParseDocument(html);

            var title = GetTitle(document);
            var volumeNumber = GetVolumeNumber(document);
            var series = GetSeries(document);
            var cover = GetCover(document);
            var release = GetReleaseDate(document);
            var publisher = GetPublisher(document);
            var status = release > DateTime.Today ? PurchaseStatus.Announced : PurchaseStatus.Wishlist;
            var type = GetBookType();
            var isbn = GetISBN(document);
            var totalVol = GetTotalVolumes(document);
            var seriesStatus = GetSeriesStatus(document);
            var originalSeriesName = GetOriginalSeriesName(document);
            var parsed = new ParsedInfo(title, GetAuthors(document), volumeNumber, series, cover, release.HasValue ? release.Value.ToString("yyyy-MM-dd") : null, publisher, type.ToString(), status.ToString(), isbn, totalVol, seriesStatus, originalSeriesName);

            return parsed;

        }

        protected abstract string GetTitle(IDocument document);
        protected abstract string GetSeries(IDocument document);
        protected abstract int GetVolumeNumber(IDocument document);
        protected abstract string GetAuthors(IDocument document);
        protected abstract string GetCover(IDocument document);
        protected abstract DateTime? GetReleaseDate(IDocument document);
        protected abstract string GetISBN(IDocument document);
        protected abstract int GetTotalVolumes(IDocument document);
        protected abstract string? GetSeriesStatus(IDocument document);
        protected abstract string? GetOriginalSeriesName(IDocument document);
        protected virtual string GetPublisher(IDocument document)
        {
            return PublisherName;
        }

        protected abstract VolumeType GetBookType();

        protected abstract string PublisherName { get; }

        protected string url;
        private readonly PublishersService _publishers;
        protected readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

        protected async Task<string> GetUrlHtml()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/128.0.0.0 Safari/537.36");
            string page = string.Empty;
            bool isForbidden = true;
            do
            {
                try
                {
                    page = await client.GetStringAsync(url);
                    isForbidden = false;
                }
                catch (Exception)
                {
                    //await Task.Delay(1000);
                    Console.WriteLine("retry");
                }
            } while (isForbidden);


            return page;
        }
    }

    public record ParsedInfo(string title, string authors, int volumeNumber, string series, string cover, string? release, string publisher, string type, string status, string isbn, int totalVolumes, string? seriesStatus, string? originalSeriesName);
}
