using ComicShelf.Models;

namespace ComicShelf.Services
{
    public class VolumeService : BasicService<Volume>
    {
        public VolumeService(ComicShelfContext context) : base(context)
        {
        }
    }
}
