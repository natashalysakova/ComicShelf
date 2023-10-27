using ComicShelf.Models;

namespace ComicShelf.Services
{
    public class FilterService : BasicService<Filter>
    {
        public FilterService(ComicShelfContext context) : base(context)
        {
        }

        public override string SetNotificationMessage()
        {
            return string.Empty;
        }
    }
}
