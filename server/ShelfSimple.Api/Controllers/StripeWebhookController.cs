using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using ShelfSimple.Api.Data;
using ShelfSimple.Api.Models;
using Stripe;

namespace ShelfSimple.Api.Controllers
{
    // Point the Stripe CLI / dashboard webhook at /api/stripe/webhook (test mode).
    [RoutePrefix("api/stripe")]
    public class StripeWebhookController : ApiController
    {
        private readonly ShelfContext db;

        public StripeWebhookController(ShelfContext db)
        {
            this.db = db;
        }

        [HttpPost, Route("webhook")]
        public async Task<IHttpActionResult> Webhook()
        {
            var json = await Request.Content.ReadAsStringAsync();
            // GetValues throws if the header isn't there at all
            IEnumerable<string> values;
            if (!Request.Headers.TryGetValues("Stripe-Signature", out values))
                return BadRequest("Missing Stripe-Signature header");
            var signature = values.FirstOrDefault();

            Event stripeEvent;
            try
            {
                stripeEvent = EventUtility.ConstructEvent(json, signature,
                    ConfigurationManager.AppSettings["stripe:WebhookSecret"]);
            }
            catch (StripeException)
            {
                return BadRequest("Bad signature");
            }

            var intent = stripeEvent.Data.Object as PaymentIntent;
            if (intent == null)
                return Ok();

            var order = await db.Orders.Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.PaymentIntentId == intent.Id);
            if (order == null || order.Status != OrderStatus.Pending)
                return Ok();

            if (stripeEvent.Type == Events.PaymentIntentSucceeded)
            {
                order.Status = OrderStatus.Paid;
                order.PaidAt = DateTime.UtcNow;

                var productIds = order.Items.Select(i => i.ProductId).ToList();
                var products = await db.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();
                foreach (var item in order.Items)
                {
                    var product = products.First(p => p.Id == item.ProductId);
                    product.Quantity = Math.Max(0, product.Quantity - item.Quantity);
                }
            }
            else if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
            {
                order.Status = OrderStatus.Failed;
            }

            await db.SaveChangesAsync();
            return Ok();
        }
    }
}
