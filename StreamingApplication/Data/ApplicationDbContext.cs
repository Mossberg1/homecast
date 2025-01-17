using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using StreamingApplication.Data.Entities;

namespace StreamingApplication.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser> {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }


    public DbSet<Media> Media { get; set; }
    public DbSet<Movie> Movie { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        _MediaConfiguration(modelBuilder);
        _MovieConfiguration(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }


    private static void _MediaConfiguration(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Media>()
            .HasIndex(e => new { e.Name, e.Path }).IsUnique();
    }


    private static void _MovieConfiguration(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Movie>()
            .HasOne(m => m.MediaFile)
            .WithOne()
            .HasForeignKey<Movie>(m => m.MediaFileId);
    }
}