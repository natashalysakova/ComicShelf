using AngleSharp.Dom;
using Backend.Models.Enums;

namespace ComicShelf.PublisherParsers
{
    public class AmazonParser : BaseParser
    {
        public override string SiteUrl => "https://www.amazon.com/";

        protected override string GetAuthors(IDocument document)
        {
            var node = document.QuerySelector("#bylineInfo");
            var authors = node.QuerySelectorAll("span .author");

            return string.Join(',', authors.Select(x => x.TextContent));
        }

        protected override VolumeType GetBookType()
        {
            return VolumeType.Digital;
        }

        protected override string GetCover(IDocument document)
        {
            var node = document.QuerySelector("#detailImg");
            if(node is null)
                return string.Empty;

            return node.GetAttribute("src");
        }

        protected override string GetISBN(IDocument document)
        {
            var nodes = document.QuerySelectorAll("#detailBullets_feature_div > ul > li");

            foreach (var node in nodes)
            {
                var data =  node.QuerySelectorAll("span > span");
                if (data[0].TextContent.Contains("ASIN"))
                {
                    return data[1].TextContent.Trim();
                }
            }

            return string.Empty;
        }

        protected override string? GetOriginalSeriesName(IDocument document)
        {
            var node = document.QuerySelector("#bylineInfo");
            return node.TextContent;
        }

        protected override string GetPublisher(IDocument document)
        {
            var node = document.QuerySelector("#rpi-attribute-book_details-publisher > .rpi-attribute-value");
            return node.TextContent.Trim();
        }

        protected override DateTime? GetReleaseDate(IDocument document)
        {
            var node = document.QuerySelector("#rpi-attribute-book_details-publication_date > .rpi-attribute-value");
            return DateTime.Parse(node.TextContent.Trim());
        }

        protected override string GetSeries(IDocument document)
        {
            var node = document.QuerySelector("#seriesBulletWidget_feature_div > a");
            return node.TextContent.Replace("Part of:", "").Trim();
        }

        protected override string? GetSeriesStatus(IDocument document)
        {
            return null;
        }

        protected override string GetTitle(IDocument document)
        {
            var node = document.QuerySelector("#productTitle");
            var title = node.TextContent;

            var lastWord = title.Split(' ', StringSplitOptions.RemoveEmptyEntries).Last();
            var isLastWordANumber = int.TryParse(lastWord, out var volume);

            if (isLastWordANumber)
            {
                return "Volume " + volume;
            }

            var indexOfNumber = title.IndexOf("Vol.") + 4;

            if (indexOfNumber == -1)
            {
                return title;
            }

            var nextWhiteSpace = title.IndexOf(' ', indexOfNumber);
            if (nextWhiteSpace == -1)
            {
                var volumeStr = title.Substring(indexOfNumber).Trim();
                return "Volume " + volumeStr;
            }
            else
            {
                var volumeStr = title.Substring(indexOfNumber, nextWhiteSpace - indexOfNumber).Trim();
                return "Volume " + volumeStr;
            }

        }

        protected override int GetTotalVolumes(IDocument document)
        {
            return -1;
        }

        protected override int GetVolumeNumber(IDocument document)
        {
            var node = document.QuerySelector("#productTitle");
            var title = node.TextContent;

            var lastWord = title.Split(' ', StringSplitOptions.RemoveEmptyEntries).Last();
            var isLastWordANumber = int.TryParse(lastWord, out var volume);

            if (isLastWordANumber)
            {
                return volume;
            }

            var indexOfNumber = title.IndexOf("Vol.") + 4;

            if (indexOfNumber == -1)
            {
                return indexOfNumber;
            }

            var nextWhiteSpace = title.IndexOf(' ', indexOfNumber);
            if (nextWhiteSpace == -1)
            {
                var volumeStr = title.Substring(indexOfNumber).Trim();
                return int.Parse(volumeStr);
            }
            else
            {
                var volumeStr = title.Substring(indexOfNumber, nextWhiteSpace - indexOfNumber).Trim();
                return int.Parse(volumeStr);
            }
        }
    }
}
