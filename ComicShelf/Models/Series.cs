using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace ComicShelf.Models
{
    public class Series
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public  string Name { get; set; }
        public string? OriginalName { get; set; }
        public bool Ongoing { get; set; }

        [Required]
        public Enums.Type Type { get; set; }

        public int TotalVolumes { get; set; }
        public bool Completed { get; set; }
        public string? Color { get; set; }
        public string? ComplimentColor { get; set; }

        public virtual ICollection<Publisher> Publishers { get; set; } = new List<Publisher>();
        public virtual ICollection<Volume> Volumes { get; set; } = new List<Volume>();

    }
}
