using ComicShelf.Localization;
using ComicShelf.Models;
using ComicShelf.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Localization;
using Org.BouncyCastle.Asn1.Ocsp;

namespace ComicShelf.Utilities
{
    public class EnumUtilities
    {
        private Dictionary<PurchaseStatus, IEnumerable<PurchaseStatus>> transitionDictionary = new Dictionary<PurchaseStatus, IEnumerable<PurchaseStatus>>
            {
                {
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
            };
        private Dictionary<PurchaseStatus, IEnumerable<SelectListItem>> transitionsCached = new Dictionary<PurchaseStatus, IEnumerable<SelectListItem>>();
        private Dictionary<string, IEnumerable<SelectListItem>> cached = new Dictionary<string, IEnumerable<SelectListItem>>();

        private readonly IStringLocalizer localizer;

        public EnumUtilities(IStringLocalizer<SharedResource> localizer)
        {
            this.localizer = localizer;
        }

        private IEnumerable<PurchaseStatus> GetPurchaseStatusesEnums(PurchaseStatus status)
        {
            return transitionDictionary[status];
        }
        private static IEnumerable<SelectListItem> GetEnumAsSelectItemList<T1>(IStringLocalizer localizer) where T1 : struct
        {
            var type = typeof(T1);
            if (!type.IsEnum)
                throw new ArgumentException($"Type {type.Name} is not an enum");

            var enums = Enum.GetNames(type);
            var values = Enum.GetValues(type);
            for (var i = 0; i < values.Length; i++)
            {
                yield return new SelectListItem { Text = localizer["@" + enums[i]], Value = values.GetValue(i).ToString() };
            }
        }


        public IEnumerable<int> GetRatings()
        {
            return Enumerable.Range(1, 5);
        }

        private IEnumerable<SelectListItem> GetSelectItemList<T>(string key, Func<IEnumerable<T>> method)
        {
            if (!cached.ContainsKey(key))
            {
                cached.Add(key, method().Select(x => new SelectListItem()
                {
                    Text = localizer["@" + x.ToString()],
                    Value = x.ToString()
                }));
            }

            return cached[key];
        }


        public IEnumerable<SelectListItem> GetPurchaseStatusesSelectItemList(PurchaseStatus status)
        {
            if (!transitionsCached.ContainsKey(status))
            {
                transitionsCached.Add(status, GetPurchaseStatusesEnums(status).Select(x => new SelectListItem()
                {
                    Text = localizer["@" + x.ToString()],
                    Value = x.ToString()
                }));
            }

            return transitionsCached[status];
        }

        public IEnumerable<SelectListItem> GetRatingsSelectItemList()
        {
            return GetSelectItemList("Ratings", GetRatings);
        }
        private IEnumerable<SelectListItem> GetSelectItemList<T>() where T: struct
        {
            if (!cached.ContainsKey(nameof(T)))
            {
                cached.Add(nameof(T), EnumUtilities.GetEnumAsSelectItemList<T>(localizer));
            }

            return cached[nameof(T)];
        }

        internal IEnumerable<SelectListItem> GetStatusSelectItemList()
        {
            return GetSelectItemList<Status>();
        }

        internal IEnumerable<SelectListItem> GetPurchaseStatusSelectItemList()
        {
            return GetSelectItemList<PurchaseStatus>();
        }

        internal IEnumerable<SelectListItem> GetDigitalitySelectItemList()
        {
            return GetSelectItemList<VolumeType>();
        }

        internal IEnumerable<SelectListItem> GetTypesSelectItemList()
        {
            return GetSelectItemList<Models.Enums.Type>();
        }

        internal IEnumerable<T> GetListOfValues<T>() where T : struct
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}