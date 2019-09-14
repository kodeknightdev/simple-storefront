using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ShelfSimple.Api.Data;
using ShelfSimple.Api.Dtos;
using ShelfSimple.Api.Models;

namespace ShelfSimple.Api.Services
{
    public class CartService
    {
        private readonly ShelfContext db;

        public CartService(ShelfContext db)
        {
            this.db = db;
        }

        public async Task<Cart> CreateAsync()
        {
            var now = DateTime.UtcNow;
            var cart = new Cart { Id = Guid.NewGuid(), CreatedAt = now, UpdatedAt = now };
            db.Carts.Add(cart);
            await db.SaveChangesAsync();
            return cart;
        }

        public Task<Cart> FindAsync(Guid cartId)
        {
            return db.Carts
                .Include(c => c.Items.Select(i => i.Product))
                .FirstOrDefaultAsync(c => c.Id == cartId);
        }

        public async Task<Cart> AddItemAsync(Guid cartId, int productId, int quantity)
        {
            var cart = await FindAsync(cartId);
            if (cart == null)
                return null;

            var product = await db.Products.FindAsync(productId);
            if (product == null || product.IsArchived)
                throw new ProductNotFoundException(productId);

            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item == null)
            {
                item = new CartItem { CartId = cartId, ProductId = productId, Quantity = 0 };
                cart.Items.Add(item);
            }

            // can't put more in the cart than the seller has
            item.Quantity = Math.Min(item.Quantity + Math.Max(1, quantity), product.Quantity);
            cart.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();

            return await FindAsync(cartId);
        }

        public async Task<Cart> SetQuantityAsync(Guid cartId, int productId, int quantity)
        {
            if (quantity <= 0)
                return await RemoveItemAsync(cartId, productId);

            var cart = await FindAsync(cartId);
            var item = cart?.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item == null)
                return cart;

            item.Quantity = Math.Min(quantity, item.Product.Quantity);
            cart.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
            return cart;
        }

        public async Task<Cart> RemoveItemAsync(Guid cartId, int productId)
        {
            var cart = await FindAsync(cartId);
            var item = cart?.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item == null)
                return cart;

            db.CartItems.Remove(item);
            cart.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
            return cart;
        }

        public static CartDto ToDto(Cart cart)
        {
            var dto = new CartDto { Id = cart.Id };
            foreach (var item in cart.Items)
            {
                dto.Items.Add(new CartItemDto
                {
                    ProductId = item.ProductId,
                    Title = item.Product.Title,
                    ImageUrl = item.Product.ImageUrl,
                    UnitPrice = item.Product.Price,
                    Quantity = item.Quantity,
                    LineTotal = item.Product.Price * item.Quantity
                });
            }
            dto.ItemCount = dto.Items.Sum(i => i.Quantity);
            dto.Subtotal = dto.Items.Sum(i => i.LineTotal);
            return dto;
        }
    }
}
