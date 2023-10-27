using Microsoft.EntityFrameworkCore;


namespace ComicShelf.Models
{
    public class ComicShelfContext : DbContext
    {
        public ComicShelfContext(DbContextOptions<ComicShelfContext> options) : base(options)
        {
        }

        public virtual DbSet<Author> Authors { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Issue> Issues { get; set; }
        public virtual DbSet<Publisher> Publishers { get; set; }
        public virtual DbSet<Series> Series { get; set; }
        public virtual DbSet<Volume> Volumes { get; set; }
        public virtual DbSet<Filter> Filters { get; set; }

        //public virtual DbSet<VolumeCover> VolumeCovers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>().ToTable("Author");
            modelBuilder.Entity<Country>().ToTable("Country");
            modelBuilder.Entity<Issue>().ToTable("Issue");
            modelBuilder.Entity<Publisher>().ToTable("Publisher");
            modelBuilder.Entity<Series>().ToTable("Series");
            modelBuilder.Entity<Volume>().ToTable("Volume");
            modelBuilder.Entity<Filter>().ToTable("Filter");
            //modelBuilder.Entity<VolumeCover>().ToTable("VolumeCovers");
        }
    }
}
