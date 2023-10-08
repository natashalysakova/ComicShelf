using ComicShelf.Localization;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Localization;

namespace ComicShelf.Utilities
{
    internal static class EnumUtilities
    {
        internal static IEnumerable<SelectListItem> GetEnumAsSelectItemList<T1>(IStringLocalizer localizer) where T1 : struct
        {
            var type = typeof(T1);
            if (!type.IsEnum)
                throw new ArgumentException($"Type {type.Name} is not an enum");

            var enums = Enum.GetNames(type);
            var values = Enum.GetValues(type);
            for (var i = 0; i < values.Length; i++)
            {
                yield return new SelectListItem { Text = localizer[enums[i]], Value = values.GetValue(i).ToString() };
            }
        }
    }
}