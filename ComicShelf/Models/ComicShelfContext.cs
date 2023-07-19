using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComicShelf.Models
{
    public class ComicShelfContext : DbContext
    {
        public ComicShelfContext(DbContextOptions<ComicShelfContext> options) : base(options)
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\natas\\source\\repos\\ComicShelf\\ComicShelf\\Data\\database.mdf;Integrated Security=True;Connect Timeout=30");
        }

        public virtual DbSet<Author> Authors { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Issue> Issues { get; set; }
        public virtual DbSet<Publisher> Publishers { get; set; }
        public virtual DbSet<Series> Series { get; set; }
        public virtual DbSet<Volume> Volumes { get; set; }
        //public virtual DbSet<AuthorVolume> AuthorVolume { get; set; }

        //public virtual DbSet<PublisherSeries> PublisherSeries { get; set; }

        [NotMapped]
        public Country UnknownCountry { get => Countries.Single(x => x.Name == "Unknown"); }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>().ToTable("Author");
            modelBuilder.Entity<Country>().ToTable("Country");
            modelBuilder.Entity<Issue>().ToTable("Issue");
            modelBuilder.Entity<Publisher>().ToTable("Publisher");
            modelBuilder.Entity<Series>().ToTable("Series");
            modelBuilder.Entity<Volume>().ToTable("Volume");
            ////modelBuilder.Entity<AuthorVolume>().ToTable("AuthorVolume");
            ////modelBuilder.Entity<PublisherSeries>().ToTable("PublisherSeries");

            //modelBuilder.Entity<AuthorVolume>(entity =>
            //{
            //    entity.HasKey(e => new { e.AuthorsId, e.VolumesId });

            //    entity.HasIndex(e => e.VolumesId, "IX_AuthorVolume_VolumesId");
            //});

            //modelBuilder.Entity<PublisherSeries>(entity =>
            //{
            //    entity.HasKey(e => new { e.PublishersId, e.SeriesId });

            //    entity.HasIndex(e => e.SeriesId, "IX_PublisherSeries_SeriesId");
            //});

        }
    }
}
