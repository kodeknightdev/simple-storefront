using System.Web.Http;
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

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
