using GithubMotivator.Models;
using Microsoft.EntityFrameworkCore;

namespace GithubMotivator.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Repository> Repositories { get; set; }
    public DbSet<Commit> Commits { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired();
        });

        modelBuilder.Entity<Repository>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.Owner, e.Name }).IsUnique();
        });

        modelBuilder.Entity<Commit>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Sha).IsUnique();
            entity.HasOne(d => d.Repository)
                .WithMany(p => p.Commits)
                .HasForeignKey(d => d.RepositoryId);
        });
    }
}