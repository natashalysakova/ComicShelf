using ComicShelf.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;

namespace ComicShelf.Services
{
    public class SearchService : IService
    {
        private ComicShelfContext _context;
        public SearchService(ComicShelfContext context)
        {
            _context = context;
        }

        public IEnumerable<string> FindPublishersByTerm(string term)
        {
            return _context.Publishers.Where(x => x.Name.ToLower().Contains(term.ToLower())).Select(x => x.Name);
        }

        internal IEnumerable<string> FindSeriesByTerm(string term)
        {
            if(string.IsNullOrEmpty(term))
                return _context.Series.Select(x => x.Name);

            var nameContains = _context.Series.Where(x => x.Name.ToLower().Contains(term.ToLower())).Select(x => x.Name);
            var originalNameContains = _context.Series.Where(x => x.OriginalName != null && x.OriginalName.ToLower().Contains(term.ToLower())).Select(x => x.Name);
            return nameContains.Union(originalNameContains).Distinct();
        }
    }
}
