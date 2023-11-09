using Backend.Models.Enums;
using ComicShelf.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.Services;
using Services.ViewModels;

namespace ComicShelf.Pages.Manga.Authors
{
    public class IndexModel : PageModel
    {
        private readonly AuthorsService _service;
        private readonly EnumUtilities _enumUtilities;

        public IndexModel(AuthorsService service, EnumUtilities enumUtilities)
        {
            _service = service;
            _enumUtilities = enumUtilities;

            AvailableRoles = _enumUtilities.GetSelectItemList<Roles>();
        }

        public IEnumerable<AuthorUpdateModel> Authors { get; set; } = default!;
        public IEnumerable<SelectListItem> AvailableRoles { get; set; }

        public void OnGetAsync()
        {
            Authors = _service.GetAllForUpdate()
                .Select(x => { x.Series = x.Series.Distinct(new IdNameViewComparer()); return x; })
                .OrderBy(x => x.Name);
        }

        public IActionResult OnPostUpdate(AuthorUpdateModel author)
        {
            if (!_service.Exists(author.Id))
            {
                return NotFound(author);
            }

            _service.Update(author);
            var updated = _service.GetForUpdate(author.Id);

            return Partial("_AuthorPartialEdit", new AuthorEditPageModel() { Author = updated, AvailableRoles = AvailableRoles });

        }
    }

    public class AuthorEditPageModel
    {
        public AuthorUpdateModel Author { get; set; }
        public IEnumerable<SelectListItem> AvailableRoles { get; set; }

    }
}
