using System;
using System.Threading.Tasks;
using System.Web.Http;
using ShelfSimple.Api.Dtos;
using ShelfSimple.Api.Services;

namespace ShelfSimple.Api.Controllers
{
    [RoutePrefix("api/carts")]
    public class CartsController : ApiController
    {
        private readonly CartService carts;

        public CartsController(CartService carts)
        {
            this.carts = carts;
        }

        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Create()
        {
            var cart = await carts.CreateAsync();
            return Created("api/carts/" + cart.Id, CartService.ToDto(cart));
        }

        [HttpGet, Route("{id:guid}")]
        public async Task<IHttpActionResult> Get(Guid id)
        {
            var cart = await carts.FindAsync(id);
            if (cart == null)
                return NotFound();
            return Ok(CartService.ToDto(cart));
        }

        [HttpPost, Route("{id:guid}/items")]
        public async Task<IHttpActionResult> AddItem(Guid id, CartItemRequest request)
        {
            Models.Cart cart;
            try
            {
                cart = await carts.AddItemAsync(id, request.ProductId, request.Quantity);
            }
            catch (ProductNotFoundException ex)
            {
                return Content(System.Net.HttpStatusCode.NotFound, new { message = ex.Message });
            }

            if (cart == null)
                return NotFound();
            return Ok(CartService.ToDto(cart));
        }

        [HttpPut, Route("{id:guid}/items/{productId:int}")]
        public async Task<IHttpActionResult> UpdateItem(Guid id, int productId, CartItemRequest request)
        {
            var cart = await carts.SetQuantityAsync(id, productId, request.Quantity);
            if (cart == null)
                return NotFound();
            return Ok(CartService.ToDto(cart));
        }

        [HttpDelete, Route("{id:guid}/items/{productId:int}")]
        public async Task<IHttpActionResult> RemoveItem(Guid id, int productId)
        {
            var cart = await carts.RemoveItemAsync(id, productId);
            if (cart == null)
                return NotFound();
            return Ok(CartService.ToDto(cart));
        }
    }
}
