using System;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security.OAuth;
using ShelfSimple.Api.Data;

namespace ShelfSimple.Api.Security
{
    // grant_type=password against the Sellers table
    public class SellerOAuthProvider : OAuthAuthorizationServerProvider
    {
        private static readonly string[] AllowedOrigins =
            (ConfigurationManager.AppSettings["cors:origins"] ?? "")
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(o => o.Trim())
                .ToArray();

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // no client ids, the only client is our own SPA
            context.Validated();
            return Task.FromResult(0);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            // /token isn't Web API so EnableCors doesn't cover it. The header can only hold a
            // single origin, so echo the caller's back if it's one we allow.
            var origin = context.Request.Headers.Get("Origin");
            if (origin != null && AllowedOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase))
                context.OwinContext.Response.Headers.Set("Access-Control-Allow-Origin", origin);

            var email = (context.UserName ?? "").Trim().ToLowerInvariant();

            using (var db = new ShelfContext())
            {
                var seller = await db.Sellers.FirstOrDefaultAsync(s => s.Email == email);
                if (seller == null || !PasswordHasher.Verify(context.Password, seller.PasswordHash))
                {
                    context.SetError("invalid_grant", "Email or password is incorrect.");
                    return;
                }

                var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, seller.Id.ToString()));
                identity.AddClaim(new Claim(ClaimTypes.Name, seller.DisplayName));
                identity.AddClaim(new Claim(ClaimTypes.Role, "Seller"));
                context.Validated(identity);
            }
        }
    }
}
