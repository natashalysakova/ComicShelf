using ComicShelf.Models;

namespace ComicShelf.Services
{
    public class AuthorsService : BasicService<Author>
    {
        public AuthorsService(ComicShelfContext context) : base(context)
        {
        }

        public override string SetNotificationMessage()
        {
            if (dbSet.Any(x => x.Roles == Models.Enums.Roles.None))
            {
                return "Some authors has no role";
            }

            return string.Empty;
        }

        public override void Update(Author author)
        {
            var tracked = Get(author.Id);
            if(tracked != null)
            {
                dbSet.Entry(tracked).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            }

            base.Update(author);
        }

        internal Author GetByName(string trimmedItem)
        {
            return dbSet.Where(x=>x.Name == trimmedItem).FirstOrDefault();
        }
    }
}
