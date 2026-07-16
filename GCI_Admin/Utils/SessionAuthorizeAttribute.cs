using GCI_Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Utils;

public class SessionAuthorizeAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var sessionService = context.HttpContext.RequestServices
            .GetService(typeof(SessionManager)) as SessionManager;

        var user = sessionService?.GetUserSession<Member>();

        if (user == null)
        {
            if (context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                (context.HttpContext.Request.Headers["Accept"].ToString()?.Contains("application/json") == true))
            {
                context.Result = new JsonResult(new { isSuccess = false, message = "Session expired", redirectToLogin = true })
                {
                    StatusCode = 401
                };
            }
            else
            {
                context.Result = new RedirectToActionResult("Index", "Auth", null);
            }
        }
    }
}