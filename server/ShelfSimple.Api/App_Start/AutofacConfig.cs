using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using ShelfSimple.Api.Data;
using ShelfSimple.Api.Services;

namespace ShelfSimple.Api
{
    public static class AutofacConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var builder = new ContainerBuilder();

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // one context per request, Autofac disposes it at the end
            builder.RegisterType<ShelfContext>().AsSelf().InstancePerRequest();
            builder.RegisterType<CartService>().AsSelf().InstancePerRequest();

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}
