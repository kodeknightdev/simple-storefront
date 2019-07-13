using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Http;
using ShelfSimple.Api.Data;
using ShelfSimple.Api.Dtos;
using ShelfSimple.Api.Models;

namespace ShelfSimple.Api.Controllers
{
    [RoutePrefix("api/products")]
    public class ProductsController : ApiController
    {
        private const int MaxPageSize = 48;

        private readonly ShelfContext db = new ShelfContext();

        // GET api/products?category=ceramics&page=1&pageSize=12
        [HttpGet, Route("")]
        public async Task<PagedResult<ProductDto>> GetProducts(string category = null, int page = 1, int pageSize = 12)
        {
            if (page < 1) page = 1;
            pageSize = Math.Max(1, Math.Min(pageSize, MaxPageSize));

            var query = db.Products.AsQueryable();
            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(p => p.Category == category);

            var items = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(ToDto)
                .ToListAsync();

            return new PagedResult<ProductDto>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = await db.Products.CountAsync()
            };
        }

        [HttpGet, Route("categories")]
        public async Task<IHttpActionResult> GetCategories()
        {
            var categories = await db.Products
                .Where(p => p.Category != null)
                .Select(p => p.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            return Ok(categories);
        }

        [HttpGet, Route("{id:int}")]
        public async Task<IHttpActionResult> GetProduct(int id)
        {
            var product = await db.Products
                .Where(p => p.Id == id)
                .Select(ToDto)
                .FirstOrDefaultAsync();

            if (product == null)
                return NotFound();

            return Ok(product);
        }

        private static readonly Expression<Func<Product, ProductDto>> ToDto = p => new ProductDto
        {
            Id = p.Id,
            Title = p.Title,
            Description = p.Description,
            ImageUrl = p.ImageUrl,
            Category = p.Category,
            Price = p.Price,
            Quantity = p.Quantity,
            SellerId = p.SellerId,
            SellerName = p.Seller.DisplayName
        };

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}
