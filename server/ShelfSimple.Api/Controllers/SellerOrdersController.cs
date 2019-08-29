using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using ShelfSimple.Api.Data;
using ShelfSimple.Api.Dtos;
using ShelfSimple.Api.Models;
using ShelfSimple.Api.Security;

namespace ShelfSimple.Api.Controllers
{
    [Authorize]
    [RoutePrefix("api/seller/orders")]
    public class SellerOrdersController : ApiController
    {
        private readonly ShelfContext db;

        public SellerOrdersController(ShelfContext db)
        {
            this.db = db;
        }

        [HttpGet, Route("")]
        public async Task<IHttpActionResult> GetMine()
        {
            var sellerId = this.GetSellerId();

            var items = await db.OrderItems
                .Include(i => i.Order)
                .Where(i => i.SellerId == sellerId && i.Order.Status == OrderStatus.Paid)
                .OrderByDescending(i => i.Order.PaidAt)
                .Take(500)
                .ToListAsync();

            var orders = items
                .GroupBy(i => i.Order)
                .Select(g => new SellerOrderDto
                {
                    OrderId = g.Key.Id,
                    BuyerEmail = g.Key.Email,
                    PaidAt = g.Key.PaidAt,
                    Total = g.Sum(i => i.UnitPrice * i.Quantity),
                    Items = g.Select(i => new SellerOrderItemDto
                    {
                        ProductId = i.ProductId,
                        Title = i.Title,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice
                    }).ToList()
                })
                .ToList();

            return Ok(orders);
        }
    }
}
