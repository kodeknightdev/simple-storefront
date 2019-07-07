using System.Data.Entity;
using ShelfSimple.Api.Models;

namespace ShelfSimple.Api.Data
{
    public class ShelfContext : DbContext
    {
        public ShelfContext() : base("name=ShelfContext")
        {
        }

        public DbSet<Seller> Sellers { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(10, 2);

            base.OnModelCreating(modelBuilder);
        }
    }
}
