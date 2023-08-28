using ComicShelf.Models;

namespace ComicShelf.Services
{
    public class AuthorsService : BasicService<Author>
    {
        public AuthorsService(ComicShelfContext context) : base(context)
        {
        }

        internal Author GetByName(string trimmedItem)
        {
            return dbSet.Where(x=>x.Name == trimmedItem).FirstOrDefault();
        }
    }
}
