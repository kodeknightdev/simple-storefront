using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ShelfSimple.Api.Data;
using ShelfSimple.Api.Dtos;
using ShelfSimple.Api.Models;
using Stripe;
using Order = ShelfSimple.Api.Models.Order;
using OrderItem = ShelfSimple.Api.Models.OrderItem;

namespace ShelfSimple.Api.Services
{
    public enum CheckoutError
    {
        None,
        CartNotFound,
        CartEmpty,
        NotEnoughStock
    }

    public class CheckoutResult
    {
        public CheckoutError Error { get; set; }
        public string Message { get; set; }
        public CheckoutResponse Response { get; set; }

        public static CheckoutResult Fail(CheckoutError error, string message = null)
        {
            return new CheckoutResult { Error = error, Message = message };
        }
    }

    // Everything order/payment related. Controllers (checkout + webhook) just translate to HTTP.
    public class CheckoutService
    {
        private readonly ShelfContext db;
        private readonly CartService carts;
        private readonly PaymentIntentService paymentIntents;

        public CheckoutService(ShelfContext db, CartService carts)
        {
            this.db = db;
            this.carts = carts;
            paymentIntents = new PaymentIntentService();
        }

        private static string Currency
        {
            get { return ConfigurationManager.AppSettings["stripe:Currency"] ?? "usd"; }
        }

        public async Task<CheckoutResult> StartAsync(Guid cartId, string email)
        {
            var cart = await carts.FindAsync(cartId);
            if (cart == null)
                return CheckoutResult.Fail(CheckoutError.CartNotFound);

            // Stripe rejects a zero amount anyway, but that came back as a 500
            if (!cart.Items.Any())
                return CheckoutResult.Fail(CheckoutError.CartEmpty, "Cart is empty");

            // stock might have changed since things went into the cart
            var soldOut = cart.Items.Where(i => i.Quantity > i.Product.Quantity).Select(i => i.Product.Title).ToList();
            if (soldOut.Any())
                return CheckoutResult.Fail(CheckoutError.NotEnoughStock, "Not enough stock for: " + string.Join(", ", soldOut));

            // If they already started checkout for this cart (went back, refreshed, card declined...)
            // reuse that PaymentIntent instead of piling up pending orders.
            var reused = await TryReusePendingAsync(cart, email);
            if (reused != null)
                return reused;

            var order = new Order
            {
                CartId = cart.Id,
                Email = email,
                Status = OrderStatus.Pending,
                Currency = Currency,
                CreatedAt = DateTime.UtcNow
            };
            foreach (var item in cart.Items)
            {
                order.Items.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    SellerId = item.Product.SellerId,
                    Title = item.Product.Title,
                    UnitPrice = item.Product.Price,
                    Quantity = item.Quantity
                });
            }
            order.Total = order.Items.Sum(i => i.UnitPrice * i.Quantity);

            db.Orders.Add(order);
            await db.SaveChangesAsync();

            var intent = await paymentIntents.CreateAsync(new PaymentIntentCreateOptions
            {
                Amount = ToCents(order.Total),
                Currency = order.Currency,
                PaymentMethodTypes = new List<string> { "card" },
                ReceiptEmail = order.Email,
                Metadata = new Dictionary<string, string>
                {
                    { "orderId", order.Id.ToString() }
                }
            });

            order.PaymentIntentId = intent.Id;
            await db.SaveChangesAsync();

            return Success(order, intent);
        }

        private async Task<CheckoutResult> TryReusePendingAsync(Cart cart, string email)
        {
            var existing = await db.Orders
                .Where(o => o.CartId == cart.Id && o.Status == OrderStatus.Pending && o.PaymentIntentId != null)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();
            if (existing == null)
                return null;

            var intent = await paymentIntents.GetAsync(existing.PaymentIntentId);
            var open = intent.Status != "succeeded" && intent.Status != "canceled";
            var total = cart.Items.Sum(i => i.Product.Price * i.Quantity);

            if (open && existing.Total == total)
            {
                existing.Email = email;
                await db.SaveChangesAsync();
                return Success(existing, intent);
            }

            // cart changed since - drop the old one and start fresh
            if (open)
                await paymentIntents.CancelAsync(existing.PaymentIntentId, new PaymentIntentCancelOptions());
            existing.Status = OrderStatus.Failed;
            await db.SaveChangesAsync();
            return null;
        }

        private static CheckoutResult Success(Order order, PaymentIntent intent)
        {
            return new CheckoutResult
            {
                Response = new CheckoutResponse
                {
                    OrderId = order.Id,
                    ClientSecret = intent.ClientSecret,
                    Total = order.Total,
                    Currency = order.Currency
                }
            };
        }

        public Task<Order> FindOrderAsync(int orderId)
        {
            return db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public Task<Order> FindByPaymentIntentAsync(string paymentIntentId)
        {
            return db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.PaymentIntentId == paymentIntentId);
        }

        // Asks Stripe for the current state of the intent, only used by /confirm.
        public async Task RefreshAsync(Order order)
        {
            if (order.Status != OrderStatus.Pending || order.PaymentIntentId == null)
                return;

            var intent = await paymentIntents.GetAsync(order.PaymentIntentId);
            if (intent.Status == "succeeded")
                await MarkPaidAsync(order);
        }

        // Safe to call twice (confirm + webhook both end up here).
        public async Task MarkPaidAsync(Order order)
        {
            if (order.Status != OrderStatus.Pending)
                return;

            order.Status = OrderStatus.Paid;
            order.PaidAt = DateTime.UtcNow;

            var productIds = order.Items.Select(i => i.ProductId).ToList();
            var products = await db.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();
            foreach (var item in order.Items)
            {
                var product = products.First(p => p.Id == item.ProductId);
                product.Quantity = Math.Max(0, product.Quantity - item.Quantity);
            }

            // the cart has done its job; if the browser still has the id it'll get a 404 and start a new one
            var cart = await db.Carts.FindAsync(order.CartId);
            if (cart != null)
                db.Carts.Remove(cart);

            await db.SaveChangesAsync();
        }

        public async Task MarkFailedAsync(Order order)
        {
            if (order.Status != OrderStatus.Pending)
                return;

            order.Status = OrderStatus.Failed;
            await db.SaveChangesAsync();
        }

        private static long ToCents(decimal amount)
        {
            return (long)Math.Round(amount * 100m, MidpointRounding.AwayFromZero);
        }
    }
}
