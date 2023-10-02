using ComicShelf.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComicShelf.Models
{
    public class Volume
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Number { get; set; }

        [Required]
        public string Title { get; set; }

        public Status Status { get; set; }
        public Rating Raiting { get; set; }
        public PurchaseStatus PurchaseStatus { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string CoverUrl { get; set; }
        public DateTime CreationDate { get; set; }

        public int SeriesId { get; set; }
        public virtual Series Series { get; set; }

        //public int CoverId { get; set; }
        //public virtual VolumeCover Cover { get; set; }

        public virtual ICollection<Author> Authors { get; set; } = new List<Author>();
        public virtual ICollection<Issue> Issues { get; set; } = new List<Issue>();


        private Dictionary<PurchaseStatus, IEnumerable<PurchaseStatus>> transitionDictionary = new Dictionary<PurchaseStatus, IEnumerable<PurchaseStatus>>
            {
                {
                    PurchaseStatus.Announced,
                    new List<PurchaseStatus>()
                    {
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
                        PurchaseStatus.Bought
                    }
                },
                {
                    PurchaseStatus.Wishlist,
                    new List<PurchaseStatus>()
                    {
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
                        PurchaseStatus.Bought
                    }
                },
                {
                    PurchaseStatus.Bought,
                    new List<PurchaseStatus>()
                    {
                        PurchaseStatus.GiftedAway
                    }
                },
                {
                    PurchaseStatus.Free,
                    new List<PurchaseStatus>()
                    {
                        PurchaseStatus.GiftedAway
                    }
                },
                {
                    PurchaseStatus.Gift,
                    new List<PurchaseStatus>()
                    {
                        PurchaseStatus.GiftedAway
                    }
                },
            };



        public IEnumerable<PurchaseStatus> GetPurchaseStatusesEnums()
        {
            return transitionDictionary[this.PurchaseStatus];
        }
        public IEnumerable<SelectListItem> GetPurchaseStatusesListItems()
        {
            var list = GetPurchaseStatusesEnums().ToList();
            list.Insert(0, PurchaseStatus);
                return list.Select(x => new SelectListItem()
                {
                    Text = x.ToString(),
                    Value = ((int)x).ToString(),
                });
        }
    }
}
