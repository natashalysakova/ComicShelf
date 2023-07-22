using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ComicShelf.Models;
using ComicShelf.Services;

namespace ComicShelf.Pages.SeriesNs
{
    public class IndexModel : PageModel
    {
        private readonly SeriesService _service;

        public IndexModel(SeriesService service)
        {
            _service = service;
        }

        public IList<Models.Series> Series { get; set; } = default!;

        public void OnGetAsync()
        {
            Series = _service.GetAll().Include("Publishers").Include("Publishers.Country").ToList();
        }
    }
}
