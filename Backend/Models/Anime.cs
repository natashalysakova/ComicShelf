using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Models
{
    public enum AnimeStatus
    {
        PlanToWatch,
        Watching,
        Finished,
        Dropped
    }

    public enum ItemStatus
    {
        PlanToWatch,
        InTheQueue,
        Watching,
        Dropped,
        Finished
    }


    public class Anime : IIdEntity
    {


        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Title { get; set; }
        public string OriginalTitle { get; set; }

        public bool IsOngoing { get; set; }
        public AnimeStatus Status { get; set; }


        public virtual ICollection<Item> Items { get; set; }
    }

    public abstract class Item : IIdEntity
    {
        public Item()
        {
            
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Title { get; set; }
        public DateTime ReleaseDate { get; set; }

        public int HierarchyOrder { get; set; }
        public int AnimeId { get; set; }
        public virtual Anime? Anime { get; set; }
        public ItemStatus Status { get; set; }

    }

    public enum ItemType
    {
        Season,
        Movie,
        Special
    }

    public class Season : Item
    {
    }
    public class Special : Item
    {
    }
    public class Movie : Item
    {
    }
}
