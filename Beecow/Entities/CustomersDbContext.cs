using Microsoft.EntityFrameworkCore;

namespace Beecow.Entities
{
    public class CustomersDbContext : DbContext
    {
        public DbSet<User> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public CustomersDbContext(DbContextOptions<CustomersDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("MST_USER");
            modelBuilder.Entity<Order>().ToTable("Order");
        }
    }
}
