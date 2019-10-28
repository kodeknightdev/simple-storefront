using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using ShelfSimple.Api.Services;
using Stripe;

namespace ShelfSimple.Api.Controllers
{
    // Point the Stripe CLI / dashboard webhook at /api/stripe/webhook (test mode).
    [RoutePrefix("api/stripe")]
    public class StripeWebhookController : ApiController
    {
        private readonly CheckoutService checkout;

        public StripeWebhookController(CheckoutService checkout)
        {
            this.checkout = checkout;
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
                // Stripe.net is pinned to one API version and throws if the event was rendered
                // with another. The webhook endpoint in the dashboard is still on the account's
                // older version, and we only read a few stable fields, so don't care.
                stripeEvent = EventUtility.ConstructEvent(json, signature,
                    ConfigurationManager.AppSettings["stripe:WebhookSecret"],
                    throwOnApiVersionMismatch: false);
            }
            catch (StripeException)
            {
                return BadRequest("Bad signature");
            }

            var intent = stripeEvent.Data.Object as PaymentIntent;
            if (intent == null)
                return Ok();

            var order = await checkout.FindByPaymentIntentAsync(intent.Id);
            if (order == null)
                return Ok();

            if (stripeEvent.Type == Events.PaymentIntentSucceeded)
                await checkout.MarkPaidAsync(order);
            else if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
                await checkout.MarkFailedAsync(order);

            return Ok();
        }
    }
}
