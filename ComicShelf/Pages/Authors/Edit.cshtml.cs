using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ComicShelf.Models;
using ComicShelf.Services;
using ComicShelf.Models.Enums;

namespace ComicShelf.Pages.Authors
{
    public class EditModel : PageModel
    {
        private readonly AuthorsService _service;

        public EditModel(AuthorsService service)
        {
            _service = service;

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
        }

        [BindProperty]
        public Author Author { get; set; } = default!;
        [BindProperty]
        public IList<Roles> SelectedRoles { get; set; }
        public IList<SelectListItem> PossibleRoles { get; set; }


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
            Author = author;
            SelectedRoles = new List<Roles>();
            var roles = Author.Roles.ToString().Split(',');

            foreach(var val in roles)
            {
                SelectedRoles.Add((Roles)Enum.Parse(typeof(Roles), val));
            }

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Author.Roles = SelectedRoles.Aggregate(Roles.None, (current, next) => current | next);

            try
            {
                _service.Update(Author);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_service.Exists(Author.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }
    }
}
