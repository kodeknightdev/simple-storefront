using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using ShelfSimple.Api.Dtos;
using ShelfSimple.Api.Services;

namespace ShelfSimple.Api.Controllers
{
    [RoutePrefix("api/checkout")]
    public class CheckoutController : ApiController
    {
        private readonly CheckoutService checkout;

        public CheckoutController(CheckoutService checkout)
        {
            this.checkout = checkout;
        }

        // Creates a pending order + PaymentIntent. The client then confirms the card
        // with Stripe.js using the client secret.
        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Checkout(CheckoutRequest request)
        {
            var result = await checkout.StartAsync(request.CartId, request.Email);
            switch (result.Error)
            {
                case CheckoutError.CartNotFound:
                    return NotFound();
                case CheckoutError.CartEmpty:
                    return BadRequest(result.Message);
                case CheckoutError.NotEnoughStock:
                    return Content(HttpStatusCode.Conflict, new { message = result.Message });
                default:
                    return Ok(result.Response);
            }
        }

        // Called by the client after stripe.handleCardPayment resolves. We don't trust the
        // client here, just use it as a nudge to go ask Stripe what happened.
        [HttpPost, Route("{orderId:int}/confirm")]
        public async Task<IHttpActionResult> Confirm(int orderId)
        {
            var order = await checkout.FindOrderAsync(orderId);
            if (order == null || order.PaymentIntentId == null)
                return NotFound();

            await checkout.RefreshAsync(order);

            return Ok(new { orderId = order.Id, status = order.Status.ToString(), total = order.Total });
        }
    }
}
