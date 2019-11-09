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
                new Models.Seller { DisplayName = "Maple & Twine", Slug = "maple-and-twine", Bio = "Knits and odds and ends from a small studio in Vermont.", Email = "maple@example.com", PasswordHash = Security.PasswordHasher.Hash("devpassword1"), CreatedAt = now },
                new Models.Seller { DisplayName = "Clayworks by June", Slug = "clayworks-by-june", Bio = "Wheel-thrown stoneware, fired in small batches.", Email = "june@example.com", PasswordHash = Security.PasswordHasher.Hash("devpassword1"), CreatedAt = now });
            context.SaveChanges();

            var maple = context.Sellers.Single(s => s.Email == "maple@example.com");
            var june = context.Sellers.Single(s => s.Email == "june@example.com");

            context.Products.AddOrUpdate(p => p.Title,
                new Models.Product { SellerId = maple.Id, Title = "Hand-knit wool beanie", Description = "Chunky merino, one size fits most.", Category = "knitwear", ImageUrl = "/img/beanie.jpg", Price = 28.00m, Quantity = 6, CreatedAt = now },
                new Models.Product { SellerId = maple.Id, Title = "Macrame plant hanger", Description = "Cotton cord, holds up to a 6\" pot.", Category = "home", ImageUrl = "/img/plant-hanger.jpg", Price = 22.50m, Quantity = 10, CreatedAt = now },
                new Models.Product { SellerId = maple.Id, Title = "Beeswax food wraps (set of 3)", Description = "Small, medium and large.", Category = "kitchen", ImageUrl = "/img/wax-wraps.jpg", Price = 18.00m, Quantity = 15, CreatedAt = now },
                new Models.Product { SellerId = june.Id, Title = "Speckled stoneware mug", Description = "12oz, dishwasher safe.", Category = "ceramics", ImageUrl = "/img/mug.jpg", Price = 32.00m, Quantity = 8, CreatedAt = now },
                new Models.Product { SellerId = june.Id, Title = "Ceramic ring dish", Description = "Glazed in sea green.", Category = "ceramics", ImageUrl = "/img/ring-dish.jpg", Price = 14.00m, Quantity = 20, CreatedAt = now },
                new Models.Product { SellerId = june.Id, Title = "Bud vase trio", Description = "Three little vases, glazes vary.", Category = "ceramics", ImageUrl = "/img/bud-vases.jpg", Price = 45.00m, Quantity = 3, CreatedAt = now });
        }
    }
}
