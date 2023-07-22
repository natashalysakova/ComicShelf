using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ComicShelf.Models;
using ComicShelf.Services;
using ComicShelf.Models.Enums;

namespace ComicShelf.Pages.Authors
{
    public class CreateModel : PageModel
    {
        private readonly AuthorsService _service;

        public CreateModel(AuthorsService service)
        {
            _service = service;
        }

        public IActionResult OnGet()
        {
            var values = Enum.GetValues(typeof(Roles));
            PossibleRoles = new List<SelectListItem>();
            foreach (var val in values)
            {
                PossibleRoles.Add(new SelectListItem()
                {
                    Text = Enum.GetName(typeof(Roles), val),
                    Value = val.ToString()
                });
            }

            return Page();
        }

        [BindProperty]
        public Author Author { get; set; } = default!;
        [BindProperty]
        public IEnumerable<Roles> SelectedRoles { get; set;}
        [BindProperty]
        public IList<SelectListItem> PossibleRoles { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || Author == null)
            {
                return Page();
            }
            Author.Roles = SelectedRoles.Aggregate(Roles.None, (current, next) => current | next);
            _service.Add(Author);

            return RedirectToPage("./Index");
        }
    }
}
