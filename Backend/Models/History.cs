using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Models
{
    public class History
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int VolumeId { get; set; }
        public DateTime? AnnouncedDate { get; set; }
        public DateTime? WishlistedDate { get; set; }
        public DateTime? PreorderedDate { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public DateTime? ReadDate { get; set; }
        public DateTime? GivedAwayDate { get; set; }
    }
}
