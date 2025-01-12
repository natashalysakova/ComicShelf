using AngleSharp;
using AngleSharp.Common;
using AngleSharp.Dom;
using AngleSharp.Html;
using AngleSharp.Html.Dom;
using HtmlAgilityPack;
using Services.Services;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace ComicShelf.PublisherParsers
{
    public interface IPublisherParser
    {
        Task<ParsedInfo> Parse();
        string SiteUrl { get; }
        void SetUrl(string url);
    }

    public record ParsedInfo(string title, string? authors, int volumeNumber, string series, string cover, string? release, string publisher, string type, string status, string isbn, int totalVolumes, string? seriesStatus, string? originalSeriesName);


    [Serializable]
    public class DocumentParseException : Exception
    {
        public IDocument Document { get; }
        public DocumentParseException(string selector, IDocument document) : base(selector)
        {
            Document = document;
        }
    }
}
