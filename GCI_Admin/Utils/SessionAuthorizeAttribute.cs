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
            context.Result = new RedirectToActionResult("Index", "Auth", null);
        }
    }
}