using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace ShelfSimple.Api.Filters
{
    // Registered globally so controllers don't each need the ModelState dance.
    // Also catches a missing/unparseable body, which Web API binds as null without an error.
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            foreach (var parameter in actionContext.ActionDescriptor.GetParameters())
            {
                if (parameter.ParameterBinderAttribute == null && !parameter.ParameterType.IsValueType
                    && parameter.ParameterType != typeof(string)
                    && actionContext.ActionArguments.ContainsKey(parameter.ParameterName)
                    && actionContext.ActionArguments[parameter.ParameterName] == null)
                {
                    actionContext.ModelState.AddModelError(parameter.ParameterName, "Request body is required.");
                }
            }

            if (!actionContext.ModelState.IsValid)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, actionContext.ModelState);
            }
        }
    }
}
