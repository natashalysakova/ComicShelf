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

        public IEnumerable<SearchResult> FindPublishersByTerm(string term)
        {
            return _context.Publishers.Where(x => x.Name.ToLower().Contains(term.ToLower())).Select(x => new SearchResult() { Label = x.Name, Value = x.Id });
        }

        internal IEnumerable<SearchResult> FindAutorByTerm(string term)
        {
            return _context.Authors.Where(x => x.Name.ToLower().Contains(term.ToLower())).Select(x => new SearchResult() { Label = x.Name, Value = x.Id });
        }

        internal IEnumerable<SearchResult> FindSeriesByTerm(string term)
        {
            if (string.IsNullOrEmpty(term))
                return _context.Series.Select(x => new SearchResult() { Label = x.Name, Value = x.Id });

            var nameContains = _context.Series
                .Where(x => x.Name.ToLower().Contains(term.ToLower()))
                .Select(x => new SearchResult() { Label = x.Name, Value = x.Id });
            var originalNameContains = _context.Series
                .Where(x => x.OriginalName != null && x.OriginalName.ToLower().Contains(term.ToLower()))
                .Select(x => new SearchResult() { Label = x.Name, Value = x.Id });
            return nameContains.Union(originalNameContains).Distinct();
        }


    }

    public class SearchResult
    {
        public int Value { get; set; }
        public string Label { get; set; }
    }
}
