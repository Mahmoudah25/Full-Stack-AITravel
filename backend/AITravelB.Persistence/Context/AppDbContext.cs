using AITravelB.Application.Common.Interfaces;
using AITravelB.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AITravelB.Persistence.Context
{
    public class AppDbContext : DbContext,IApplicationDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<Trip> Trips => Set<Trip>();
        public DbSet<Activity> Activities => Set<Activity>();
        public DbSet<Rating> Ratings => Set<Rating>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}