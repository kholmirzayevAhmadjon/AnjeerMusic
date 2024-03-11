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
}