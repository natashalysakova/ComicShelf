using Backend.Models.Enums;
using Microsoft.AspNetCore.Http;

namespace Services.ViewModels
{
    public class VolumeUpdateModel : IUpdateModel
    {
        public int Id { get; set; }
        public PurchaseStatus PurchaseStatus { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime ReleaseDate { get; set; }
        public DateTime PreorderDate { get; set; }
        public Status Status { get; set; }
        public int Rating { get; set; }
        public IFormFile? CoverFile { get; set; }
    }
}
