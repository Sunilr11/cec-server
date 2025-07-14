using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using GDT.CEC.Repository.Models.Response;

namespace GDT.CEC.Web.Middleware
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SecuredApiAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous)
            {
                return;
            }
            object isUserAuthenticated = false;
            context.HttpContext.Items.TryGetValue("IsUserAuthenticated", out isUserAuthenticated);
            if ((bool)isUserAuthenticated == false)
            {
                var message = JsonSerializer.Serialize(new ReturnResponse<string> { Message = "Invalid Token", Data = null });
                context.Result = new JsonResult(message) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }
    }
}
