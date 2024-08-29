using Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ViewModels
{
    public class HistoryViewModel : IViewModel<History>
    {
        public string? AnnouncedDate { get; set; }
        public string? WishlistedDate { get; set; }
        public string? PreorderedDate { get; set; }
        public string? ReleaseDate { get; set; }
        public string? PurchaseDate { get; set; }
        public string? ReadDate { get; set; }
        public string? GivedAwayDate { get; set; }

    }
}
