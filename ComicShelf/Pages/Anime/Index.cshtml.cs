using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using AnimeModel = Backend.Models.Anime;

namespace ComicShelf.Pages.Anime
{
    public class IndexModel : PageModel
    {
        private readonly ComicShelfContext _context;

        public IndexModel(ComicShelfContext context)        {
            _context = context;
        }
        public ICollection<Season> Seasons { get; set; }
        public ICollection<Movie> Movies { get; set; }
        public ICollection<Special> Specials { get; set; }
        public ICollection<AnimeModel> Anime { get; set; }


        public void OnGet()
        {
            Seasons = _context.Items.OfType<Season>().ToList();
            Movies = _context.Items.OfType<Movie>().ToList();
            Specials = _context.Items.OfType<Special>().ToList();

            Anime = _context.Anime.ToList();
        }
    }
}
