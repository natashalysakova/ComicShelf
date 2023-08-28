using ComicShelf.Models;

namespace ComicShelf.Services
{
    public class CoverService : BasicService<VolumeCover>
    {
        public CoverService(ComicShelfContext context) : base(context)
        {
        }
    }
}
