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
    public class DetailsModel : PageModel
    {
        private readonly PublishersService _publisherService;

        public DetailsModel(PublishersService service)
        {
            _publisherService = service;
        }

      public Publisher Publisher { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publisher = _publisherService.GetWithCountry(id);
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
    }
}
