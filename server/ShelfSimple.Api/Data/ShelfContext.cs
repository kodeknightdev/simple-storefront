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
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(10, 2);

            // don't take a seller's product down with someone's abandoned cart
            modelBuilder.Entity<CartItem>()
                .HasRequired(i => i.Product)
                .WithMany()
                .HasForeignKey(i => i.ProductId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<OrderItem>()
                .HasRequired(i => i.Product)
                .WithMany()
                .HasForeignKey(i => i.ProductId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Order>()
                .Property(o => o.Total)
                .HasPrecision(10, 2);

            modelBuilder.Entity<OrderItem>()
                .Property(i => i.UnitPrice)
                .HasPrecision(10, 2);

            base.OnModelCreating(modelBuilder);
        }
    }
}
