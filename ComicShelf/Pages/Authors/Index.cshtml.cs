using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ComicShelf.Models;
using ComicShelf.Services;

namespace ComicShelf.Pages.Authors
{
    public class IndexModel : PageModel
    {
        private readonly AuthorsService _service;

        public IndexModel(AuthorsService service)
        {
            _service = service;
        }

        public IList<Author> Author { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Author = _service.GetAll().OrderBy(x => x.Name).ToList(); ;
        }
    }
}
