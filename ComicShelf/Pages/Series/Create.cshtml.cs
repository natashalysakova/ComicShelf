using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ComicShelf.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ComicShelf.Pages.SeriesNs
{
    public class CreateModel : PageModel
    {
        private readonly Models.ComicShelfContext _context;

        public CreateModel(Models.ComicShelfContext context)
        {
            _context = context;
            AvailablePublishers = _context.Publishers.OrderBy(x => x.Name).Select(x => new SelectListItem(x.Name, x.Id.ToString()));
            var enums = Enum.GetNames(typeof(Models.Enums.Type));
            var values = Enum.GetValues(typeof(Models.Enums.Type));
            for (var i = 0; i < values.Length; i++)
            {
                Types.Add(new SelectListItem { Text = enums[i], Value = values.GetValue(i).ToString() });
            }
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public SeriesModel Series { get; set; } = default!;

        public IEnumerable<SelectListItem> AvailablePublishers { get; set; }
        public List<SelectListItem> Types { get; set; } = new List<SelectListItem>();


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            //if (Publishers != null)
            //{
            //    var newPublisher = _context.Publishers.Add(new Publisher { Name = Publishers, Country = _context.Countries.Single(x => x.Name == "None") });
            //    Series.PublishersIds.Add(newPublisher.Entity.Id.ToString());
            //    ModelState.Clear();
            //    TryValidateModel(ModelState);
            //}

            if (!ModelState.IsValid || _context.Series == null || Series == null)
            {
                return Page();
            }

            _context.Series.Add(Series.ToModel(_context));
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        public IActionResult OnGetSearch(string term)
        {
            return new JsonResult(_context.Publishers.Where(x => x.Name.Contains(term)).Select(x => x.Name));
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class RequiredIfAttribute : ValidationAttribute, IClientModelValidator
    {
        public string PropertyName { get; set; }
        public object Value { get; set; }

        public RequiredIfAttribute(string propertyName, object value, string errorMessage = "")
        {
            PropertyName = propertyName;
            ErrorMessage = errorMessage;
            Value = value;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var instance = validationContext.ObjectInstance;
            var type = instance.GetType();
            var proprtyvalue = type.GetProperty(PropertyName).GetValue(instance, null);
            if (proprtyvalue != null)
            {
                if (proprtyvalue.ToString() == Value.ToString() && value == null)
                {
                    return new ValidationResult(ErrorMessage);
                }
            }
            return ValidationResult.Success;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val", "true");
            var errorMessage = FormatErrorMessage(context.ModelMetadata.GetDisplayName());
            MergeAttribute(context.Attributes, "data-val-reqif", errorMessage);
        }

        private bool MergeAttribute(
        IDictionary<string, string> attributes,
        string key,
        string value)
        {
            if (attributes.ContainsKey(key))
            {
                return false;
            }
            attributes.Add(key, value);
            return true;
        }
    }
}
