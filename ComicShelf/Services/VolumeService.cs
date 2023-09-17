using ComicShelf.Models;
using ComicShelf.Models.Enums;
using ComicShelf.Pages.Volumes;
using Microsoft.Extensions.Hosting.Internal;
using System.Security.Policy;
using UnidecodeSharpFork;
using static System.Net.Mime.MediaTypeNames;
using System.Web;
using ComicType = ComicShelf.Models.Enums.Type;

namespace ComicShelf.Services
{
    public class VolumeService : BasicService<Volume>
    {
        private readonly SeriesService _seriesService;
        private readonly AuthorsService _authorService;
        private readonly CoverService _coverService;

        public VolumeService(ComicShelfContext context, SeriesService seriesService, AuthorsService authorService, CoverService coverService) : base(context)
        {
            _seriesService = seriesService;
            _authorService = authorService;
            _coverService = coverService;
        }

        public void Add(VolumeModel model)
        {
            var series = _seriesService.GetByName(model.Series);
            if (series == null)
            {
                series = new Series() { Name = model.Series, Type = ComicType.Comics };
                _seriesService.Add(series);
            }

            var authorList = new List<Author>();
            foreach (var item in model.Authors)
            {
                var trimmedItem = item.Trim();
                if (string.IsNullOrEmpty(trimmedItem))
                {
                    continue;
                }

                var author = _authorService.GetByName(trimmedItem);
                if (author == null)
                {
                    Roles role;

                    if (series.Type == ComicType.Manga)
                    {
                        role = Roles.Mangaka;
                    }
                    else
                    {
                        role = Roles.None;
                    }

                    author = new Author() { Name = trimmedItem, Roles = role };
                    _authorService.Add(author);
                }

                authorList.Add(author);
            }

            var issues = new List<Issue>(model.Issues);

            var urlPath = string.Empty;
            var cover = new VolumeCover();
            if (model.CoverFile != null && model.CoverFile.Length > 0)
            {

                //urlPath = FileUtility.DownloadFileFromWeb(model.Cover, series.Name, model.Number, out byte[] coverBytes, out string extention);
                urlPath = FileUtility.SaveOnServer(model.CoverFile, series.Name, model.Number, out byte[] coverBytes, out string extention);
                cover.Cover = coverBytes;
                cover.Extention = extention;
            }

            var volume = new Volume()
            {
                Title = model.Title,
                Issues = issues,
                Series = series,
                Authors = authorList,
                Number = model.Number,
                PurchaseDate = model.PurchaseDate,
                PurchaseStatus = model.PurchaseStatus,
                Raiting = model.Raiting,
                Status = model.Status,
                CoverUrl = urlPath,
                Cover = cover,
                CreationDate = DateTime.Now,
            };

            this.Add(volume);
        }

        internal string GetSeriesName(Volume volume)
        {
            return _seriesService.GetAll().Single(x => x.Volumes.Contains(volume)).Name;
        }

        public override void Update(Volume item)
        {
            //if (item.CoverUrl.StartsWith("http"))
            //{
            //    var seriesName = GetSeriesName(item);
            //    item.CoverUrl = FileUtility.SaveOnServer(model.CoverFile, seriesName, item.Number, out byte[] coverBytes, out string extention);

            //    var cover = _coverService.GetCoverForVolume(item);
            //    if(cover == null)
            //    {
            //        var original = Get(item.Id);
            //        cover = new VolumeCover() { Cover = coverBytes, Extention = extention };
            //        original.Cover = cover;
            //        _coverService.Add(cover);
            //    }
            //    else
            //    {
            //        cover.Cover = coverBytes;
            //        cover.Extention = extention;
            //        _coverService.Update(cover);
            //    }
            //}

            base.Update(item);
        }
    }
}
