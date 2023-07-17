using System.ComponentModel.DataAnnotations.Schema;

namespace ComicShelf.Models
{
    public class Author
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Volume> Volumes { get; set; } = new List<Volume>();
    }
}
