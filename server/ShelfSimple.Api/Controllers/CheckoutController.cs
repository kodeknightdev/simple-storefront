using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using ShelfSimple.Api.Data;
using ShelfSimple.Api.Dtos;
using ShelfSimple.Api.Models;
using ShelfSimple.Api.Services;
using Stripe;
using Order = ShelfSimple.Api.Models.Order;
using OrderItem = ShelfSimple.Api.Models.OrderItem;

namespace ShelfSimple.Api.Controllers
{
    [RoutePrefix("api/checkout")]
    public class CheckoutController : ApiController
    {
        private readonly ShelfContext db;
        private readonly CartService carts;

        public CheckoutController(ShelfContext db, CartService carts)
        {
            this.db = db;
            this.carts = carts;
        }

        // Creates a pending order + PaymentIntent. The client then confirms the card
        // with Stripe.js using the client secret.
        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Checkout(CheckoutRequest request)
        {
            if (request == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            var cart = await carts.FindAsync(request.CartId);
            if (cart == null)
                return NotFound();

            var currency = ConfigurationManager.AppSettings["stripe:Currency"] ?? "usd";

            var order = new Order
            {
                CartId = cart.Id,
                Email = request.Email,
                Status = OrderStatus.Pending,
                Currency = currency,
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

            var intentService = new PaymentIntentService();
            var intent = await intentService.CreateAsync(new PaymentIntentCreateOptions
            {
                Amount = (long)(order.Total * 100),
                Currency = currency,
                PaymentMethodTypes = new List<string> { "card" },
                ReceiptEmail = order.Email,
                Metadata = new Dictionary<string, string>
                {
                    { "orderId", order.Id.ToString() }
                }
            });

            order.PaymentIntentId = intent.Id;
            await db.SaveChangesAsync();

            return Ok(new CheckoutResponse
            {
                OrderId = order.Id,
                ClientSecret = intent.ClientSecret,
                Total = order.Total,
                Currency = currency
            });
        }
    }
}
