using LogisticAssistantMinimalAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LogisticAssistantMinimalAPI
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users => Set<User>();
        public DbSet<Truck> Trucks => Set<Truck>();
        public DbSet<TruckRoute> Routes => Set<TruckRoute>();
    }
}
