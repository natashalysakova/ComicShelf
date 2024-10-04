
using Backend.Models.Enums;
using ComicShelf.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services;

namespace ComicShelf.Utilities
{
    public class EnumUtilities
    {
        private Dictionary<PurchaseStatus, IEnumerable<PurchaseStatus>> transitionDictionary = new Dictionary<PurchaseStatus, IEnumerable<PurchaseStatus>>
            {{
                PurchaseStatus.Announced,
                new List<PurchaseStatus>()
                {
                    PurchaseStatus.Announced,
                    PurchaseStatus.Preordered,
                    PurchaseStatus.Bought,
                    PurchaseStatus.Pirated,
                    PurchaseStatus.Wishlist,
                    PurchaseStatus.Free
                }
            },
            {
                PurchaseStatus.Preordered,
                new List<PurchaseStatus>()
                {
                    PurchaseStatus.Preordered,
                    PurchaseStatus.Bought
                }
            },
            {
                PurchaseStatus.Wishlist,
                new List<PurchaseStatus>()
                {
                    PurchaseStatus.Wishlist,
                    PurchaseStatus.Preordered,
                    PurchaseStatus.Bought,
                    PurchaseStatus.Pirated,
                    PurchaseStatus.Gift,
                    PurchaseStatus.Free
                }
            },
            {
                PurchaseStatus.Pirated,
                new List<PurchaseStatus>()
                {
                    PurchaseStatus.Pirated,
                    PurchaseStatus.Bought
                }
            },
            {
                PurchaseStatus.Bought,
                new List<PurchaseStatus>()
                {
                    PurchaseStatus.Bought,
                    PurchaseStatus.GiftedAway
                }
            },
            {
                PurchaseStatus.Free,
                new List<PurchaseStatus>()
                {
                    PurchaseStatus.Free,
                    PurchaseStatus.GiftedAway
                }
            },
            {
                PurchaseStatus.Gift,
                new List<PurchaseStatus>()
                {
                    PurchaseStatus.Gift,
                    PurchaseStatus.GiftedAway
                }
            },
            {
                PurchaseStatus.GiftedAway,
                new List<PurchaseStatus>()
                {
                    PurchaseStatus.GiftedAway, PurchaseStatus.Wishlist, PurchaseStatus.Bought, PurchaseStatus.Pirated, PurchaseStatus.Gift, PurchaseStatus.Free
                }
            }};
        private Dictionary<PurchaseStatus, IEnumerable<SelectListItem>> transitionsCached = new Dictionary<PurchaseStatus, IEnumerable<SelectListItem>>();
        private Dictionary<string, IEnumerable<SelectListItem>> cached = new Dictionary<string, IEnumerable<SelectListItem>>();

        private readonly ILocalizationService localizer;

        public EnumUtilities(ILocalizationService localizer)
        {
            this.localizer = localizer;
        }

        private IEnumerable<PurchaseStatus> GetPurchaseStatusesEnums(PurchaseStatus status)
        {
            return transitionDictionary[status];
        }
        public IEnumerable<SelectListItem> GetPurchaseStatusesSelectItemList(PurchaseStatus status)
        {
            if (!transitionsCached.ContainsKey(status))
            {
                transitionsCached.Add(status, GetPurchaseStatusesEnums(status).Select(x => new SelectListItem()
                {
                    Text = localizer["" + x.ToString()],
                    Value = x.ToString()
                }));
            }

            return transitionsCached[status];
        }

        private static IEnumerable<SelectListItem> GetEnumAsSelectItemList<T1>(ILocalizationService localizer) where T1 : struct
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

        public IEnumerable<SelectListItem> GetSelectItemList<T>() where T : struct
        {
            var typeName = typeof(T).Name;
            if (!cached.ContainsKey(typeName))
            {
                cached.Add(typeName, GetEnumAsSelectItemList<T>(localizer));
            }

            return cached[typeName];
        }



        public IEnumerable<int> GetRatings()
        {
            return Enumerable.Range(1, 5);
        }

        internal IEnumerable<T> GetListOfValues<T>() where T : struct
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
        public IEnumerable<SelectListItem> GetRatingsSelectItemList()
        {
            return GetSelectItemList("Ratings", GetRatings);
        }
        private IEnumerable<SelectListItem> GetSelectItemList<T>(string key, Func<IEnumerable<T>> method)
        {
            if (!cached.ContainsKey(key))
            {
                cached.Add(key, method().Select(x => new SelectListItem()
                {
                    Text = localizer["" + x.ToString()],
                    Value = x.ToString()
                }));
            }

            return cached[key];
        }
    }
}