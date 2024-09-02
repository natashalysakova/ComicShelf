using AngleSharp.Browser;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Backend.Models;
using Backend.Models.Enums;

namespace ComicShelf.PublisherParsers
{
    public class NashaIdeaParser : BaseParser
    {


        public NashaIdeaParser(string url, IConfiguration configuration) : base(url, configuration)
        {
        }

        protected override string PublisherName => "NashaIdea";

        protected override string GetAuthors(IDocument document)
        {
            return string.Empty;
        }

        protected override string GetCover(IDocument document)
        {
            var node = document.QuerySelectorAll(".woocommerce-product-gallery__image > a").First();
            var attribute = node.Attributes["href"];
            return attribute.Value;
        }

        protected override string GetSeries(IDocument document)
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
                var series = title.Substring(0, volIndex).Trim([' ', ',', '.']);
                return series;
            }
        }

        protected override string GetTitle(IDocument document)
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

        protected override int GetVolumeNumber(IDocument document)
        {
            var tag = document.QuerySelector("h1.product_title");
            var title = tag.InnerHtml;

            var volIndex = title.IndexOf("Том");

            if (volIndex == -1)
                return volIndex;

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

        protected override DateTime? GetReleaseDate(IDocument document)
        {
            var nodes = document.QuerySelectorAll(".book-product-description > p");
            string? releaseNode = null;
            foreach (var node in nodes)
            {
                if (node.TextContent.StartsWith("У НАЯВНОСТІ"))
                {
                    releaseNode = node.TextContent;
                }
            }

            if (releaseNode is null)
                return null;

            var date = releaseNode.Replace("У НАЯВНОСТІ", "").Trim([' ', '.', '.', '!']);
            var dateString = GetDateFromText(date);

            return dateString;
        }

        private DateTime? GetDateFromText(string text)
        {
            if (text.Contains('–'))
            {
                text = text.Substring(text.LastIndexOf('–') + 1).Trim([' ', '.', '.', '!']);
            }

            string? month = default;
            int day = 0;
            if (text.StartsWith("У "))
            {
                month = text.Substring(2);
            }
            if (text.StartsWith("ПЕРША ПОЛОВИНА"))
            {
                month = text.Substring("ПЕРША ПОЛОВИНА".Length);
                day = 15;
            }
            if (text.StartsWith("ДРУГА ПОЛОВИНА"))
            {
                month = text.Substring("ДРУГА ПОЛОВИНА".Length);
            }

            if (month == default)
                return null;

            month = month.Trim([' ', '.', '.', '!']);
            var monthNumber = monthes.SingleOrDefault(x => x.names.Contains(month));

            if (monthNumber == default)
                return null;

            var year = DateTime.Today.Month > monthNumber.number ? DateTime.Today.Year + 1 : DateTime.Today.Year;

            if (day == 0)
                day = DateTime.DaysInMonth(year, monthNumber.number);


            return new DateTime(year, monthNumber.number, day);
        }

        protected override VolumeType GetBookType()
        {
            return VolumeType.Physical;
        }

        protected override string GetISBN(IDocument document)
        {
            var node = document.QuerySelector(".book-product-table-ibn");
            var text = node.TextContent.Substring(node.TextContent.IndexOf(":") + 1).Trim();
            return text;
        }

        protected override int GetTotalVolumes(IDocument document)
        {
            var nodes = document.QuerySelectorAll(".book-product-table-data-weight");

            //var volumeNode = nodes.SingleOrDefault(x => x.TextContent.Contains("Кількість томів"));

            //if (volumeNode != null)
            //{
            //    return int.Parse(volumeNode.TextContent.Substring(volumeNode.TextContent.IndexOf(':') + 1).Trim());
            //}

            var oldNode = nodes.SingleOrDefault(x => x.TextContent.Contains("Статус серії"));

            var split = oldNode.TextContent.Split([',', '.', ' ', '-'], StringSplitOptions.RemoveEmptyEntries);
            foreach (var item2 in split)
            {
                if (item2.Contains("Однотомник"))
                {
                    return 1;
                }

                if(item2 == "завершена")
                {
                    var volumeNode = nodes.SingleOrDefault(x => x.TextContent.Contains("Кількість томів"));

                    if (volumeNode != null)
                    {
                        return int.Parse(volumeNode.TextContent.Substring(volumeNode.TextContent.IndexOf(':') + 1).Trim());
                    }
                }

                int.TryParse(item2.Trim(), out var volume);
                if (volume != 0)
                {
                    return volume;
                }
            }

            return -1;
        }

        protected override string? GetSeriesStatus(IDocument document)
        {
            var node = document.QuerySelectorAll(".book-product-table-data-weight");
            foreach (var item in node)
            {
                if (item.TextContent.Contains("Статус серії:"))
                {
                    if (item.TextContent.Contains("Однотомник"))
                    {
                        return "oneshot";
                    }

                    if (item.TextContent.Contains("Серія незавершена"))
                    {
                        return "ongoing";
                    }

                    return "finished";
                }
            }
            return null;
        }

        protected override string? GetOriginalSeriesName(IDocument document)
        {
            return null;
        }

        (int number, string[] names)[] monthes = [
            (1, ["СІЧЕНЬ", "СІЧНІ", "СІЧНЯ"]),
            (2, ["ЛЮТИЙ", "ЛЮТОМУ", "ЛЮТОГО"]),
            (3, ["БЕРЕЗЕНЬ", "БЕРЕЗНІ", "БЕРЕЗНЯ"]),
            (4, ["КВІТЕНЬ", "КВІТНІ", "КВТНЯ"]),
            (5, ["ТРАВЕНЬ", "ТРАВНІ", "ТРАВНЯ"]),
            (6, ["ЧЕРВЕНЬ", "ЧЕВНІ", "ЧЕРВНЯ"]),
            (7, ["ЛИПЕНЬ", "ЛИПНІ", "ЛИПНЯ"]),
            (8, ["СЕРПЕНЬ", "СЕРПНІ", "СЕРПНЯ"]),
            (9, ["ВЕРЕСЕНЬ", "ВЕРЕСНІ", "ВЕРЕСНЯ"]),
            (10, ["ЖОВТЕНЬ", "ЖОВТНІ", "ЖОВТНЯ"]),
            (11, ["ЛИСТОПАД", "ЛИСТОПАДІ", "ЛИСТОПАДА"]),
            (12, ["ГРУДЕНЬ", "ГРУДНІ", "ГРУДНЯ"]),
            ];
    }
}
