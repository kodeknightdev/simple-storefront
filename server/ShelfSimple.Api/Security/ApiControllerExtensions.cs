using System.Security.Claims;
using System.Web.Http;

namespace ShelfSimple.Api.Security
{
    public static class ApiControllerExtensions
    {
        public static int GetSellerId(this ApiController controller)
        {
            var identity = controller.User.Identity as ClaimsIdentity;
            var claim = identity?.FindFirst(ClaimTypes.NameIdentifier);
            return claim == null ? 0 : int.Parse(claim.Value);
        }
    }
}
