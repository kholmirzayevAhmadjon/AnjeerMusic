using AnjeerMusic.Domain.Configurations;
using AnjeerMusic.Domain.Musics;
using AnjeerMusic.Domain.UserMusics;
using AnjeerMusic.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace AnjeerMusic.Data.DbContexts;

public class AppDbContext:DbContext
{
    public DbSet<User> users {  get; set; }
    public DbSet<Music> musicals { get; set; }
    public DbSet<UserMusic> userMusicals { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(Constants.ConnectionString);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserMusic>()
            .HasKey(um => new { um.UserId, um.MusicId });

        modelBuilder.Entity<UserMusic>()
            .HasOne(user => user.User)
            .WithMany(m => m.userMusics)
            .HasForeignKey(um => um.UserId);

        modelBuilder.Entity<UserMusic>()
            .HasOne(music => music.Music)
            .WithMany()
            .HasForeignKey(um => um.MusicId);
    }
}