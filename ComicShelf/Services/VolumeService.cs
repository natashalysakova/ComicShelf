using ComicShelf.Models;
using ComicShelf.Models.Enums;
using ComicShelf.Pages.Volumes;
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
            var series = _seriesService.Get(int.Parse(model.Series));
            if(series == null)
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

                var author = _authorService.Get(int.Parse(trimmedItem));
                if (author == null)
                {
                    Roles role;

                    if(series.Type == ComicType.Manga)
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
            };

            if (model.Cover != null)
            {
                var cover = new VolumeCover() { Cover = model.Cover };
                _coverService.Add(cover);
                volume.Cover = cover;
            }

            this.Add(volume);

        }
    }
}
