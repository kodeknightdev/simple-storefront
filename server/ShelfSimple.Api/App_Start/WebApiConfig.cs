using System.Configuration;
using System.Web.Http;
using System.Web.Http.Cors;
using Newtonsoft.Json.Serialization;

namespace ShelfSimple.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // JSON only, camelCase so the frontend doesn't have to deal with PascalCase
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver =
                new CamelCasePropertyNamesContractResolver();

            // angular dev server runs on :4200
            var origins = ConfigurationManager.AppSettings["cors:origins"];
            config.EnableCors(new EnableCorsAttribute(origins, "*", "*"));

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
