using AutoMapper;
using Backend.Models;
using Backend.Models.Enums;
using Services.ViewModels;

namespace Services.Services
{
    public class AuthorsService : BasicService<Author, AuthorViewModel, AuthorCreateModel, AuthorUpdateModel>
    {
        public AuthorsService(ComicShelfContext context, IMapper mapper) : base(context,mapper)
        {
        }

        public override string SetNotificationMessage()
        {
            if (dbSet.Any(x => x.Roles == Roles.None))
            {
                return "Some authors has no role";
            }

            return string.Empty;
        }


        public IQueryable<AuthorViewModel> GetAll()
        {
            return dbSet.Select(x=> _mapper.Map<AuthorViewModel>(x));
        }

        internal Author GetByName(string trimmedItem)
        {
            return dbSet.Where(x => x.Name == trimmedItem).FirstOrDefault();
        }

        internal ICollection<Author> GetAll(List<int> authorList)
        {
            var collection = new List<Author>();
            foreach(var author in authorList)
            {
                collection.Add(dbSet.Find(author));
            }
            return collection;
        }




    }
}
