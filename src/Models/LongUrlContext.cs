using Microsoft.EntityFrameworkCore;
using urlshortener.Models;

namespace urlshortener.Models
{
    public class LongUrlContext : DbContext
    {
        public LongUrlContext(DbContextOptions<LongUrlContext> options) : base(options) {}
        public DbSet<LongUrl> longUrls { get; set; }

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);

            builder.Entity<LongUrl>().ToTable("url");
            builder.Entity<LongUrl>().HasKey(p => p.Id);
            builder.Entity<LongUrl>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<LongUrl>().Property(p => p.Url).IsRequired();
        }
    }
}