using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ComicShelf.Models;
using ComicShelf.Services;

namespace ComicShelf.Pages.Publishers
{
    public class IndexModel : PageModel
    {
        private readonly PublishersService _publisherService;

        public IndexModel(PublishersService service)
        {
            _publisherService = service;
        }

        public IList<Publisher> Publisher { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Publisher = _publisherService.GetAll().Include(x=>x.Country).Include(x=>x.Series).OrderByDescending(x=>x.Series.Count).ThenBy(x=>x.Name).ToList();
        }
    }
}
