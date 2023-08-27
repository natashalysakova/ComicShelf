using Microsoft.AspNetCore.Mvc.Rendering;

namespace ComicShelf.Pages.Volumes
{
    internal class Utilities
    {
        internal static IEnumerable<SelectListItem> GetEnumAsSelectItemList(Type type)
        {
            if(!type.IsEnum)
                throw new ArgumentException($"Type {type.Name} is not an enum");

            var enums = Enum.GetNames(type);
            var values = Enum.GetValues(type);
            for (var i = 0; i < values.Length; i++)
            {
                yield return new SelectListItem { Text = enums[i], Value = values.GetValue(i).ToString() };
            }
        }
    }
}