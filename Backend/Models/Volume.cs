﻿using Backend.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public partial class Volume : IIdEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Number { get; set; }

        [Required]
        public string Title { get; set; }

        public Status Status { get; set; }
        public int Rating { get; set; }
        public PurchaseStatus PurchaseStatus { get; set; }
        public VolumeType Digitality { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public string CoverUrl { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ModificationDate { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public DateTime? PreorderDate { get; set; }

        public bool OneShot { get; set; }
        public bool SingleIssue { get; set; }

        public string? ISBN { get; set; }

        public int SeriesId { get; set; }
        public virtual Series Series { get; set; }

        public virtual History History { get; set; }

        public virtual ICollection<Author> Authors { get; set; } = new List<Author>();
        public virtual ICollection<Issue> Issues { get; set; } = new List<Issue>();

        public bool Expired()
        {
            return
                (PurchaseStatus == PurchaseStatus.Announced || PurchaseStatus == PurchaseStatus.Preordered) &&
                ReleaseDate.HasValue && ReleaseDate < DateTime.Today;
        }
    }
}
