using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Http;
using ShelfSimple.Api.Data;
using ShelfSimple.Api.Dtos;
using ShelfSimple.Api.Models;
using ShelfSimple.Api.Security;

namespace ShelfSimple.Api.Controllers
{
    [RoutePrefix("api/sellers")]
    public class SellersController : ApiController
    {
        private readonly ShelfContext db;

        public SellersController(ShelfContext db)
        {
            this.db = db;
        }

        [HttpPost, Route("register")]
        public async Task<IHttpActionResult> Register(RegisterSellerRequest request)
        {
            if (request == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            var email = request.Email.Trim().ToLowerInvariant();
            if (await db.Sellers.AnyAsync(s => s.Email == email))
                return Conflict();

            var seller = new Seller
            {
                DisplayName = request.DisplayName.Trim(),
                Email = email,
                PasswordHash = PasswordHasher.Hash(request.Password),
                CreatedAt = DateTime.UtcNow
            };
            db.Sellers.Add(seller);
            await db.SaveChangesAsync();

            return Created("api/sellers/" + seller.Id, new SellerDto
            {
                Id = seller.Id,
                DisplayName = seller.DisplayName,
                Email = seller.Email
            });
        }
    }
}
