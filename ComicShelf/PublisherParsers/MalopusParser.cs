using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Backend.Models.Enums;
using NuGet.Common;
using System.Security.Policy;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace ComicShelf.PublisherParsers
{
    public class MalopusParser : BaseParser
    {
        public override string SiteUrl => "https://malopus.com.ua/";

        public override Task<string> GetUrlHtml(string url)
        {
            return this.GetUrlHtml(url, true);
        }

        protected override async Task<Dictionary<string, string>?> PreRequest(string url, CancellationToken token = default)
        {
            var html = await this.GetUrlHtml(url, false);
            if (html.Contains("defaultHash"))
            {
                var hashIndex = html.IndexOf("defaultHash");
                var hashStart = html.IndexOf("\"", hashIndex) + 1;
                var hashEnd = html.IndexOf("\"", hashStart);
                var hash = html.Substring(hashStart, hashEnd - hashStart);

                return new Dictionary<string, string>
                {
                    { "Cookie", $"challenge_passed={hash}; max-age=1800; path=/; samesite=Lax" }
                };
            }
            return null;
        }

        private string? GetFromTable(IDocument document, string headerText)
        {
            var nodes = document.QuerySelectorAll(".product-features__row");

            foreach (var item in nodes)
            {
                var header = item.QuerySelector("th > span");

                if (header is not null && header.TextContent.Contains(headerText))
                {
                    var value = item.QuerySelector("td");
                    return value?.TextContent.Trim();
                }
            }

            return string.Empty;
        }

        protected override string? GetAuthors(IDocument document)
        {
            return GetFromTable(document, "Автор");
        }

        protected override string GetCover(IDocument document)
        {
            var node = document.QuerySelector(".gallery__photo-img");
            var attribute = node.Attributes["src"];
            return this.SiteUrl + attribute.Value.TrimStart('/');
        }

        protected override DateTime? GetReleaseDate(IDocument document)
        {
            var html = document.ToHtml();

            int index = html.IndexOf("Орієнтовна дата надходження:");
            if (index == -1)
                return null;


            var date = html.Substring(index + "Орієнтовна дата надходження:".Length + 1, 10);

            if (date == "0000-00-00")
                return null;


            if (DateTime.TryParseExact(date, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.AssumeLocal, out DateTime parsedExactDate))
            {
                return parsedExactDate;
            }

            if (DateTime.TryParse(date, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeLocal, out DateTime parsedDate))
            {
                return parsedDate;
            }

            return null;
        }

        protected override string GetSeries(IDocument document)
        {
            var node = document.QuerySelector(".product-title");

            var title = node.TextContent;
            title = ReplaceVolumeType(title);

            var lookupChar = new char[] { '.', '!', '?' };
            int index = -1;
            foreach (var ch in lookupChar)
            {
                index = title.IndexOf(ch);
                if (index != -1)
                {
                    break;
                }
            }

            if (index != -1)
            {
                title = title.Substring(0, index).Trim();
            }

            return title;
        }

        protected override string GetVolumeTitle(IDocument document)
        {
            var node = document.QuerySelector(".product-title");

            var title = node.TextContent;

            var lookupChar = new char[] { '.', '!', '?' };
            int index = -1;
            foreach (var ch in lookupChar)
            {
                index = node.InnerHtml.IndexOf(ch);
                if (index != -1)
                {
                    break;
                }
            }

            if (index != -1)
            {
                title = title.Substring(index + 1).Trim();
            }

            title = ReplaceVolumeType(title);

            return title;
        }

        private static string ReplaceVolumeType(string title)
        {
            if (title.StartsWith("Ранобе") || title.StartsWith("Манґа") || title.StartsWith("Комікс") || title.StartsWith("Передзамовлення"))
            {
                title = title.Substring(title.IndexOf(' ') + 1).Trim();
            }

            return title;
        }

        string[] lookupArray = [". Том ", "! Том ", "? Том ", ". Омнібус ", "! Омнібус ", "? Омнібус "];

        protected override int GetVolumeNumber(IDocument document)
        {
            var node = document.QuerySelector(".product-title");
            var title = node.InnerHtml;

            if (!lookupArray.Any(x => title.Contains(x)))
            {
                return -1;
            }

            var lookupValue = lookupArray.Single(x => title.Contains(x));

            int indexOfVolume, nextWhitespace;
            indexOfVolume = title.IndexOf(lookupValue) + lookupValue.Length;
            nextWhitespace = title.IndexOf(' ', indexOfVolume);
            string volume;
            if (nextWhitespace == -1)
            {
                volume = title.Substring(indexOfVolume).Trim();
            }
            else
            {
                volume = title.Substring(indexOfVolume, nextWhitespace - indexOfVolume);
            }
            return int.Parse(volume);
        }

        protected override VolumeType GetVolumeType()
        {
            return VolumeType.Physical;
        }

        protected override string GetISBN(IDocument document)
        {
            return GetFromTable(document, "ISBN") ?? string.Empty;
        }

        protected override int GetTotalVolumes(IDocument document)
        {
            var text = GetFromTable(document, "Кількість томів");
            if (text is null)
                return -1;

            if (text.Contains('/'))
            {
                return int.Parse(text.Split('/', StringSplitOptions.RemoveEmptyEntries).First());
            }
            else if (text.Contains('(') && text.Contains(')'))
            {
                var indexopen = text.IndexOf('(') + 1;
                var indexclose = text.IndexOf(')');
                return int.Parse(text.Substring(indexopen, indexclose - indexopen));
            }
            else if (int.TryParse(text, out int totalVolumes))
            {
                return totalVolumes;
            }
            else
            {
                return GetVolumeNumber(document);
            }
        }

        protected override string GetSeriesStatus(IDocument document)
        {
            var text = GetFromTable(document, "Кількість томів");

            if (text.Contains("онґоїнґ"))
            {
                return "ongoing";
            }
            else if (text == "1")
            {
                return "oneshot";
            }
            else
            {
                return "finished";
            }
        }

        protected override string? GetOriginalSeriesName(IDocument document)
        {
            return GetFromTable(document, "Оригінальна назва");
        }

        protected override string GetPublisher(IDocument document)
        {
            return "Mal'opus";
        }
    }
}
