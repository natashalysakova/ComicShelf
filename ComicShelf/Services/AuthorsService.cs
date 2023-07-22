using ComicShelf.Models;

namespace ComicShelf.Services
{
    public class AuthorsService : BasicService<Author>
    {
        public AuthorsService(ComicShelfContext context) : base(context)
        {
        }


    }
}
