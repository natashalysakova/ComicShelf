using Backend.Models.Enums;
using ComicShelf.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Services.Services;
using Services.ViewModels;

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

        public IList<AuthorViewModel> Author { get; set; } = default!;

        public void OnGetAsync()
        {
            ViewData["AvailableRoles"] = _enumUtilities.GetSelectItemList<Roles>();
            Author = _service.GetAll().OrderBy(x => x.Name).ToList(); ;
        }

        public IActionResult OnPostUpdate(AuthorUpdateModel author)
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
