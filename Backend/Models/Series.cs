﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class Series : IIdEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string? OriginalName { get; set; }
        public bool Ongoing { get; set; }

        [Required]
        public Enums.Type Type { get; set; }
        public int MalId { get; set; }

        public int TotalVolumes { get; set; }
        public int TotalIssues { get; set; }
        public bool Completed { get; set; }
        public string? Color { get; set; }
        public string? ComplimentColor { get; set; }

        public int PublisherId { get; set; }
        public virtual Publisher? Publisher { get; set; }

        public virtual ICollection<Volume> Volumes { get; set; } = new List<Volume>();
    }
}
