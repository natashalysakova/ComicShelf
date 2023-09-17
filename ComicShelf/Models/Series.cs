using System.ComponentModel.DataAnnotations.Schema;

namespace ComicShelf.Models
{
    public class Series
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? OriginalName { get; set; }
        public bool Ongoing { get; set; }

        public required Enums.Type Type { get; set; }

        public int TotalIssues { get; set; }
        public int HasIssues { get; set; }
        public bool Completed { get; set; }
        public string Color { get; set; }
        public string ComplimentColor { get; set; }

        public virtual ICollection<Publisher> Publishers { get; set; } = new List<Publisher>();
        public virtual ICollection<Volume> Volumes { get; set; } = new List<Volume>();
    }
}
