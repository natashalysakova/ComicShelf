using Backend.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.Web.CodeGeneration.Design;
using MySqlConnector;


namespace Backend.Models
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
        public virtual DbSet<History> History { get; set; }

        public virtual DbSet<Filter> Filters { get; set; }
        public virtual DbSet<Anime> Anime { get; set; }
        public virtual DbSet<Item> Items { get; set; }


        //public virtual DbSet<VolumeCover> VolumeCovers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>().ToTable("Author");
            modelBuilder.Entity<Country>().ToTable("Country");
            modelBuilder.Entity<Issue>().ToTable("Issue").HasDiscriminator<string>("issue_type").HasValue("chapter");
            modelBuilder.Entity<Bonus>().ToTable("Issue").HasDiscriminator<string>("issue_type").HasValue("bonus");
            modelBuilder.Entity<Publisher>().ToTable("Publisher");
            modelBuilder.Entity<Series>().ToTable("Series");
            modelBuilder.Entity<Volume>().ToTable("Volume");
            modelBuilder.Entity<Filter>().ToTable("Filter");
            //modelBuilder.Entity<VolumeCover>().ToTable("VolumeCovers");



            modelBuilder.Entity<Anime>().ToTable("Animes");

            //modelBuilder.Entity<Item>().ToTable("Items")
            //    .HasDiscriminator(x => x.Type);
            modelBuilder.Entity<Movie>().ToTable("Items")
                .HasDiscriminator<string>("item_type")
                .HasValue("movie");
            modelBuilder.Entity<Special>().ToTable("Items")
                .HasDiscriminator<string>("item_type")
                .HasValue("special");
            modelBuilder.Entity<Season>().ToTable("Items")
                .HasDiscriminator<string>("item_type")
                .HasValue("season");

            

        }
    }

    public class BoberDbContextFactory : IDesignTimeDbContextFactory<ComicShelfContext>
    {
        public ComicShelfContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("devsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ComicShelfContext>();

            string? connectionString = configuration.GetConnectionString("db");
            if (connectionString == null)
            {
                throw new NullReferenceException(nameof(connectionString));
            }
            var serverVersion = GetServerVersion(connectionString);
            _ = optionsBuilder.UseMySql(connectionString, serverVersion);
            return new ComicShelfContext(optionsBuilder.Options);
        }

        private static ServerVersion GetServerVersion(string? connectionString)
        {
            ServerVersion? version = default;

            do
            {
                try
                {
                    Console.WriteLine("connecting to " + connectionString);
                    version = ServerVersion.AutoDetect(connectionString);
                    Console.WriteLine("Success");
                }
                catch (MySqlException ex)
                {
                    if (ex.Message.Contains("Unable to connect to any of the specified MySQL hosts"))
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("Trying in 5 seconds");
                        Thread.Sleep(5000);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            while (version is null);
            return version;
        }
    }
}
