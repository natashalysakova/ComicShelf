using Microsoft.Extensions.Localization;

namespace Services
{
    public interface ILocalizationService
    {
         LocalizedString this[string name]
        {
            get;
        }
        LocalizedString this[string name, params object[] arguments]
        {
            get;
        }
        IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures);
    }
}