using ComicShelf.Models;

namespace ComicShelf.Services
{
    public class CoverService : BasicService<VolumeCover>
    {
        public CoverService(ComicShelfContext context) : base(context)
        {
        }

        internal VolumeCover GetCoverForVolume(Volume item)
        {
            return context.VolumeCovers.SingleOrDefault(x=>x.Volume.Id == item.Id);
        }
    }
}
