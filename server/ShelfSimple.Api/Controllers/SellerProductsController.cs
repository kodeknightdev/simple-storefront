using System;
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
    [RoutePrefix("api/seller/products")]
    public class SellerProductsController : ApiController
    {
        private readonly ShelfContext db;

        public SellerProductsController(ShelfContext db)
        {
            this.db = db;
        }

        [HttpGet, Route("")]
        public async Task<IHttpActionResult> GetMine()
        {
            var sellerId = this.GetSellerId();
            var products = await db.Products
                .Where(p => p.SellerId == sellerId && !p.IsArchived)
                .OrderBy(p => p.Title)
                .ToListAsync();

            return Ok(products.Select(ToDto));
        }

        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Create(ProductInput input)
        {
            if (input == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            var product = new Product { SellerId = this.GetSellerId(), CreatedAt = DateTime.UtcNow };
            Apply(product, input);
            db.Products.Add(product);
            await db.SaveChangesAsync();

            return Created("api/products/" + product.Id, ToDto(product));
        }

        [HttpPut, Route("{id:int}")]
        public async Task<IHttpActionResult> Update(int id, ProductInput input)
        {
            if (input == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            var product = await db.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            Apply(product, input);
            await db.SaveChangesAsync();
            return Ok(ToDto(product));
        }

        [HttpDelete, Route("{id:int}")]
        public async Task<IHttpActionResult> Archive(int id)
        {
            var product = await db.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            product.IsArchived = true;
            await db.SaveChangesAsync();
            return StatusCode(System.Net.HttpStatusCode.NoContent);
        }

        private static void Apply(Product product, ProductInput input)
        {
            product.Title = input.Title.Trim();
            product.Description = input.Description;
            product.ImageUrl = input.ImageUrl;
            product.Category = string.IsNullOrWhiteSpace(input.Category) ? null : input.Category.Trim().ToLowerInvariant();
            product.Price = input.Price;
            product.Quantity = input.Quantity;
        }

        private static ProductDto ToDto(Product p)
        {
            return new ProductDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                ImageUrl = p.ImageUrl,
                Category = p.Category,
                Price = p.Price,
                Quantity = p.Quantity,
                SellerId = p.SellerId
            };
        }
    }
}
