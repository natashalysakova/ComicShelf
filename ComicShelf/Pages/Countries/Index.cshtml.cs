using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ComicShelf.Models;
using ComicShelf.Services;
using System.Globalization;
using System.Diagnostics.Metrics;
using System.Drawing;

namespace ComicShelf.Pages.Countries
{
    public class IndexModel : PageModel
    {
        private readonly CountryService _service;

        public IndexModel(CountryService service)
        {
            _service = service;
        }

        public List<Country> Countries { get;set; } = default!;

        public void OnGet()
        {
            Countries = _service.GetAll().Include(x => x.Publishers).Where(x => x.Publishers.Any()).ToList();
        }
    }
}
