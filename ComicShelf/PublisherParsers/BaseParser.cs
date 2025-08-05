using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Backend.Models.Enums;
using PuppeteerSharp;

namespace ComicShelf.PublisherParsers
{
    public abstract class BaseParser : IPublisherParser
    {
        protected const int maxretry = 10;

        private string url;
        public void SetUrl(string url)
        {
            this.url = url;
        }

        public async Task<ParsedInfo> Parse()
        {
            //var config = new Configuration().WithDefaultLoader();
            //var document = await BrowsingContext.New(config).OpenAsync(url);
            var html = await GetUrlHtml(url);
            var parser = new HtmlParser();
            var document = parser.ParseDocument(html);

            try
            {
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
                var authors = GetAuthors(document);

                var parsed = new ParsedInfo(title, authors, volumeNumber, series, cover, release.HasValue ? release.Value.ToString("yyyy-MM-dd") : null, publisher, type.ToString(), status.ToString(), isbn, totalVol, seriesStatus, originalSeriesName);
                return parsed;
            }
            catch (Exception)
            {
                Console.WriteLine(html);
                throw;
            }



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
        protected abstract string GetPublisher(IDocument document);

        protected abstract VolumeType GetBookType();

        public abstract string SiteUrl { get; }

        protected virtual async Task<string> GetUrlHtml(string url)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Clear();

            //client.DefaultRequestHeaders.Add("Accept-language", "en-GB,en;q=0.9");
            //client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            //client.DefaultRequestHeaders.Add("Cache-Control", "max-age=0");
            //client.DefaultRequestHeaders.Add("Connection", "keep-alive");

            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/128.0.0.0 Safari/537.36");
            int retry = 0;
            do
            {
                try
                {
                    var page = await client.GetStringAsync(url);
                    return page;
                }
                catch (Exception)
                {
                    await Task.Delay(1000);
                    Console.WriteLine("retry");
                    retry += 1;
                }
            } while (retry < maxretry);

            throw new Exception("Cannot access website");
        }

    }

    public abstract class AdvancedParser : BaseParser
    {
        override protected async Task<string> GetUrlHtml(string url)
        {
            int retry = 0;
            do
            {
                try
                {
                    var browserFetcher = new BrowserFetcher();
                    await browserFetcher.DownloadAsync();

                    await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
                    {
                        Headless = true,
                        Args = new[] { "--no-sandbox" },
                    });

                    await using var page = await browser.NewPageAsync();
                    await page.SetUserAgentAsync("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/138.0.0.0 Safari/537.36");
                    await page.SetExtraHttpHeadersAsync(new Dictionary<string, string>
                    {
                        { "Accept-Language", "uk,uk-UA;q=0.9,en-US;q=0.8,en;q=0.7,de;q=0.6" }
                    });

                    await page.GoToAsync(url, WaitUntilNavigation.Networkidle0);
                    var content = await page.GetContentAsync();
                    return content;
                }
                catch (Exception ex)
                {
                    await Task.Delay(1000);
                    Console.WriteLine($"Error accessing {url}: {ex.Message}");
                    Console.WriteLine("retry");
                    retry += 1;
                }
            } while (retry < maxretry);

            throw new Exception("Cannot access website");
        }
    }
}
