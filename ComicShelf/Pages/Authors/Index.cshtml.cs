using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ComicShelf.Models;
using ComicShelf.Services;
using ComicShelf.Utilities;
using ComicShelf.Models.Enums;

namespace ComicShelf.Pages.Authors
{
    public class IndexModel : PageModel
    {
        private readonly AuthorsService _service;
        private readonly EnumUtilities _enumUtilities;

        public IndexModel(AuthorsService service, EnumUtilities enumUtilities)
        {
            _service = service;
            _enumUtilities = enumUtilities;
        }

        public IList<Author> Author { get; set; } = default!;

        public async Task OnGetAsync()
        {
            ViewData["AvailableRoles"] = _enumUtilities.GetSelectItemList<Roles>();
            Author = _service.GetAll().Include(x=>x.Volumes).ThenInclude(x=>x.Series).OrderBy(x => x.Name).ToList(); ;
        }

        public async Task<IActionResult> OnPostUpdate(Author author)
        {
            if(!_service.Exists(author.Id))
            {
                return NotFound(author);
            }

            _service.Update(author);

            return StatusCode(200);
        }
    }
}
