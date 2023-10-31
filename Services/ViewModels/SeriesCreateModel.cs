using Backend.Models;
using Services.Utilities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Services.ViewModels
{
    public class SeriesCreateModel : ICreateModel
    {
        public string Name { get; set; }
        public Backend.Models.Enums.Type Type { get; set; }
        public int TotalVolumes { get;  set; }
        public bool Completed { get;  set; }
        public bool Ongoing { get;  set; }
    }
}
