using Microsoft.Extensions.Localization;

namespace ComicShelf.Localization
{
    public class LocalizationService
    {
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly ILogger _logger;

        public LocalizationService(IStringLocalizer<SharedResource> localizer, ILogger<LocalizationService> logger)
        {
            _localizer = localizer;
            _logger = logger;
        }

        public LocalizedString this[string name]
        {
            get
            {
                return CheckResult(_localizer.GetString(name));
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                return CheckResult(_localizer.GetString(name, arguments));
            }
        }

        private LocalizedString CheckResult(LocalizedString result)
        {
            if (result.ResourceNotFound || string.IsNullOrEmpty(result.Value))
            {
                result = new LocalizedString(result.Name, "{" + result.Name + "}", result.ResourceNotFound, result.SearchedLocation);
                _logger.LogWarning($"Translation for '{result.Name}' {(result.ResourceNotFound ? "not found" : "is empty")}. Fallback value {result.Value}");
            }
            return result;
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return _localizer.GetAllStrings(includeParentCultures);
        }
    }
}
