using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectLigaNosWeb.Data.Entities;

namespace ProjectLigaNosWeb.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DbSet<Clubs> Clubs { get; set; }

        //public DbSet<User> Users {  get; set; }
        public DbSet<Players> Players { get; set; }
        public DbSet<Games> Games { get; set; }
        public DbSet<Statistics> Statistics { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Clubs>(entity =>
            {
                entity.HasMany(c => c.HomeGames)
                      .WithOne(g => g.ClubHome)
                      .HasForeignKey(g => g.ClubHomeId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(c => c.AwayGames)
                      .WithOne(g => g.ClubAway)
                      .HasForeignKey(g => g.ClubAwayId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(c => c.Statistics)
                      .WithOne(s => s.Club)
                      .HasForeignKey(s => s.ClubId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(c => c.Players)
                      .WithOne(p => p.Club)
                      .HasForeignKey(p => p.ClubId)
                      .OnDelete(DeleteBehavior.Restrict); 
            });

            modelBuilder.Entity<Players>(entity =>
            {
                entity.HasOne(p => p.Club)
                      .WithMany(c => c.Players)
                      .HasForeignKey(p => p.ClubId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Games>(entity =>
            {
                entity.HasMany(g => g.Statistics)
                      .WithOne(s => s.Game)
                      .HasForeignKey(s => s.GameId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }



    }
}
 