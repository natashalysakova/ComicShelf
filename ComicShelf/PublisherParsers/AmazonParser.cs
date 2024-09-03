﻿using AngleSharp.Dom;
using Backend.Models.Enums;

namespace ComicShelf.PublisherParsers
{
    public class AmazonParser : BaseParser
    {
        public override string SiteUrl => "https://www.amazon.com/";

        protected override string GetAuthors(IDocument document)
        {
            var node = document.QuerySelector("#bylineInfo");
            var authors = node.QuerySelectorAll("span.author");

            var selected = new List<string>();

            foreach (var auth in authors)
            {
                var text = auth.TextContent;
                if (text.Contains("(Translator)"))
                {
                    continue;
                }

                text = text.Replace(['\t', '\n'], "");

                var desc = text.IndexOf('(');
                if (desc == -1)
                {
                    selected.Add(text);
                }
                else
                {
                    selected.Add(text.Substring(0, desc));
                }
            }

            return string.Join(',', selected.Select(x => x.Trim()));
        }

        protected override VolumeType GetBookType()
        {
            return VolumeType.Digital;
        }

        protected override string GetCover(IDocument document)
        {
            var node = document.QuerySelector("#landingImage");
            if (node is null)
                return string.Empty;

            return node.GetAttribute("data-old-hires");
        }

        protected override string GetISBN(IDocument document)
        {
            var nodes = document.QuerySelectorAll("#detailBullets_feature_div > ul > li");

            foreach (var node in nodes)
            {
                var data = node.QuerySelectorAll("span > span");
                if (data[0].TextContent.Contains("ASIN"))
                {
                    return data[1].TextContent.Trim();
                }
            }

            return string.Empty;
        }

        protected override string? GetOriginalSeriesName(IDocument document)
        {
            return null;
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
            var node = document.QuerySelector("#productTitle") ?? throw new DocumentParseException("#productTitle", document);


            var title = node.TextContent;

            var words = title.Split([' '], StringSplitOptions.RemoveEmptyEntries);

            for (var i = 0; i < words.Length; i++)
            {
                if (words[i] == "Vol." && i < words.Length - 1)
                {
                    return "Volume " + words[i + 1];
                }
            }

            return title.Trim();
        }

        protected override int GetTotalVolumes(IDocument document)
        {
            return -1;
        }

        protected override int GetVolumeNumber(IDocument document)
        {
            var node = document.QuerySelector("#productTitle");
            var title = node.TextContent;

            //var lastWord = title.Split(' ', StringSplitOptions.RemoveEmptyEntries).Last();
            //var isLastWordANumber = int.TryParse(lastWord, out var volume);

            //if (isLastWordANumber)
            //{
            //    return volume;
            //}




            var words = title.Split([' '], StringSplitOptions.RemoveEmptyEntries);

            for (var i = 0; i < words.Length; i++)
            {
                if (words[i] == "Vol." && i < words.Length - 1)
                {
                    return int.Parse(words[i + 1]);
                }
            }

            return -1;


            //var indexOfNumber = title.IndexOf("Vol.");

            //if (indexOfNumber == -1)
            //{
            //    return indexOfNumber;
            //}
            //indexOfNumber += "Vol.".Length + 1;

            //var nextWhiteSpace = title.IndexOf(' ', indexOfNumber);
            //if (nextWhiteSpace == -1)
            //{
            //    var volumeStr = title.Substring(indexOfNumber).Trim();
            //    return int.Parse(volumeStr);
            //}
            //else
            //{
            //    var volumeStr = title.Substring(indexOfNumber, nextWhiteSpace - indexOfNumber).Trim();
            //    return int.Parse(volumeStr);
            //}
        }
    }
}
