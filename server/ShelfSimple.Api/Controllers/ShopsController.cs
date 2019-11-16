using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using ShelfSimple.Api.Data;
using ShelfSimple.Api.Dtos;

namespace ShelfSimple.Api.Controllers
{
    [RoutePrefix("api/shops")]
    public class ShopsController : ApiController
    {
        private readonly ShelfContext db;

        public ShopsController(ShelfContext db)
        {
            this.db = db;
        }

        [HttpGet, Route("{slug}")]
        public async Task<IHttpActionResult> Get(string slug)
        {
            var seller = await db.Sellers.FirstOrDefaultAsync(s => s.Slug == slug);
            if (seller == null)
                return NotFound();

            var products = await db.Products
                .Where(p => p.SellerId == seller.Id && !p.IsArchived)
                .OrderByDescending(p => p.CreatedAt)
                .Select(ProductsController.ToDto)
                .ToListAsync();

            return Ok(new ShopDto
            {
                Slug = seller.Slug,
                DisplayName = seller.DisplayName,
                Bio = seller.Bio,
                Products = products
            });
        }
    }
}
