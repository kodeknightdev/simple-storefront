namespace ShelfSimple.Api.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ShelfSimple.Api.Data.ShelfContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ShelfSimple.Api.Data.ShelfContext context)
        {
            // Dev data only - a couple of sellers and some products to click around with.
            var now = DateTime.UtcNow;

            context.Sellers.AddOrUpdate(s => s.Email,
                new Models.Seller { DisplayName = "Maple & Twine", Email = "maple@example.com", CreatedAt = now },
                new Models.Seller { DisplayName = "Clayworks by June", Email = "june@example.com", CreatedAt = now });
            context.SaveChanges();

            var maple = context.Sellers.Single(s => s.Email == "maple@example.com");
            var june = context.Sellers.Single(s => s.Email == "june@example.com");

            context.Products.AddOrUpdate(p => p.Title,
                new Models.Product { SellerId = maple.Id, Title = "Hand-knit wool beanie", Description = "Chunky merino, one size fits most.", Price = 28.00m, Quantity = 6, CreatedAt = now },
                new Models.Product { SellerId = maple.Id, Title = "Macrame plant hanger", Description = "Cotton cord, holds up to a 6\" pot.", Price = 22.50m, Quantity = 10, CreatedAt = now },
                new Models.Product { SellerId = maple.Id, Title = "Beeswax food wraps (set of 3)", Description = "Small, medium and large.", Price = 18.00m, Quantity = 15, CreatedAt = now },
                new Models.Product { SellerId = june.Id, Title = "Speckled stoneware mug", Description = "12oz, dishwasher safe.", Price = 32.00m, Quantity = 8, CreatedAt = now },
                new Models.Product { SellerId = june.Id, Title = "Ceramic ring dish", Description = "Glazed in sea green.", Price = 14.00m, Quantity = 20, CreatedAt = now },
                new Models.Product { SellerId = june.Id, Title = "Bud vase trio", Description = "Three little vases, glazes vary.", Price = 45.00m, Quantity = 3, CreatedAt = now });
        }
    }
}
