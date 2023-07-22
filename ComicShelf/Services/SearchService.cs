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
    }
}
