using System.Configuration;
using System.Web.Http;
using Stripe;

namespace ShelfSimple.Api
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            AutofacConfig.Register(GlobalConfiguration.Configuration);

            StripeConfiguration.ApiKey = ConfigurationManager.AppSettings["stripe:SecretKey"];
        }
    }
}
