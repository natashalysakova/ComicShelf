﻿using System.ComponentModel.DataAnnotations.Schema;

namespace ComicShelf.Models
{
    public class Issue
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public required string Name { get; set; }
        public int Number { get; set; }

        public Volume Volume { get; set; }
    }
}