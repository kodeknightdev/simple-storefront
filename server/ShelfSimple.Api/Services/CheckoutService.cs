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
