using Backend.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class Author : IIdEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public Roles Roles { get; set; }

        public virtual ICollection<Volume> Volumes { get; set; } = new List<Volume>();
    }
}
