using ComicShelf.Localization;
using ComicShelf.Models;
using ComicShelf.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;

namespace ComicShelf.Utilities
{
    internal static class VolumeUtilities
    {
        private static Dictionary<PurchaseStatus, IEnumerable<PurchaseStatus>> transitionDictionary = new Dictionary<PurchaseStatus, IEnumerable<PurchaseStatus>>
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
        public static IEnumerable<PurchaseStatus> GetPurchaseStatusesEnums(Volume volume)
        {
            return transitionDictionary[volume.PurchaseStatus];
        }
        public static IEnumerable<SelectListItem> GetPurchaseStatusesSelectItemList(Volume volume, IStringLocalizer localizer)
        {
            return GetPurchaseStatusesEnums(volume).Select(x => new SelectListItem()
            {
                Text = localizer[x.ToString()],
                Value = x.ToString()
            });
        }

        public static IEnumerable<SelectListItem> GetRatingsSelectItemList(IStringLocalizer localizer)
        {
            return GetRatings().Select(x => new SelectListItem()
            {
                Text = localizer[x.ToString()],
                Value = x.ToString()
            });
        }
        public static IEnumerable<int> GetRatings()
        {
            return Enumerable.Range(1, 5);
        }

        internal static IEnumerable<SelectListItem> GetStatusSelectItemList(IStringLocalizer localizer)
        {
            return EnumUtilities.GetEnumAsSelectItemList<Status>(localizer);
        }

        internal static IEnumerable<SelectListItem> GetPurchaseStatusSelectItemList(IStringLocalizer localizer)
        {
            return EnumUtilities.GetEnumAsSelectItemList<PurchaseStatus>(localizer);
        }


    }
    internal static class SeriesUtilities
    {
        internal static IEnumerable<SelectListItem> GetTypesSelectItemList(IStringLocalizer localizer)
        {
            return EnumUtilities.GetEnumAsSelectItemList<PurchaseStatus>(localizer);
        }
    }
}