using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class Filter : IIdEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Json { get; set; }
        public string Group { get; set; }
        public int DisplayOrder { get; set; }
    }
}
