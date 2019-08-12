using System;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using ShelfSimple.Api.Security;

[assembly: OwinStartup(typeof(ShelfSimple.Api.Startup))]

namespace ShelfSimple.Api
{
    // OWIN is only here for the token endpoint + bearer auth, Web API itself
    // is still hosted the normal way through Global.asax.
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/token"),
                Provider = new SellerOAuthProvider(),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(7),
                // IIS Express is https already, this is just so plain http works locally too
                AllowInsecureHttp = true
            });

            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }
    }
}
