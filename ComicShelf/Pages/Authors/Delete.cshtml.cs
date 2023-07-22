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
    public class DeleteModel : PageModel
    {
        private readonly AuthorsService _service;

        public DeleteModel(AuthorsService service)
        {
            _service = service;
        }

        [BindProperty]
      public Author Author { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = _service.Get(id);

            if (author == null)
            {
                return NotFound();
            }
            else 
            {
                Author = author;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var author = _service.Get(id);

            if (author != null)
            {
                Author = author;
                _service.Remove(Author);
            }

            return RedirectToPage("./Index");
        }
    }
}
