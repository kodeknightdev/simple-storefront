using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using ShelfSimple.Api.Data;
using ShelfSimple.Api.Dtos;

namespace ShelfSimple.Api.Controllers
{
    [RoutePrefix("api/products")]
    public class ProductsController : ApiController
    {
        private readonly ShelfContext db = new ShelfContext();

        [HttpGet, Route("")]
        public async Task<IEnumerable<ProductDto>> GetProducts()
        {
            return await db.Products
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    SellerId = p.SellerId,
                    SellerName = p.Seller.DisplayName
                })
                .ToListAsync();
        }

        [HttpGet, Route("{id:int}")]
        public async Task<IHttpActionResult> GetProduct(int id)
        {
            var product = await db.Products
                .Where(p => p.Id == id)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    SellerId = p.SellerId,
                    SellerName = p.Seller.DisplayName
                })
                .FirstOrDefaultAsync();

            if (product == null)
                return NotFound();

            return Ok(product);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}
