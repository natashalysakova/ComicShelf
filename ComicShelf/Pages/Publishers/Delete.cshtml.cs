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
    public class DeleteModel : PageModel
    {
        private readonly PublishersService _publisherService;

        public DeleteModel(ComicShelfContext context, PublishersService service)
        {
            _publisherService = service;
        }

        [BindProperty]
      public Publisher Publisher { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publisher = _publisherService.Get(id);

            if (publisher == null)
            {
                return NotFound();
            }
            else 
            {
                Publisher = publisher;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var publisher = _publisherService.Get(id);

            if (publisher != null)
            {
                Publisher = publisher;
                _publisherService.Remove(publisher);
            }

            return RedirectToPage("./Index");
        }
    }
}
