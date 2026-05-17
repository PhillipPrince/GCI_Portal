using GCI_Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Utils;

namespace GCI_Admin.Utils
{
    public class PermissionAuthorizeAttribute : ActionFilterAttribute
    {
        private readonly string _permission;

        public PermissionAuthorizeAttribute(string permission)
        {
            _permission = permission;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var sessionService = context.HttpContext.RequestServices
                .GetService(typeof(SessionManager)) as SessionManager;

            if (sessionService == null)
            {
                context.Result = new RedirectToActionResult("Index", "Auth", null);
                return;
            }

            // Check if user is logged in via session
            var isLoggedIn = sessionService.IsUserLoggedIn();

            if (!isLoggedIn)
            {
                context.Result = new RedirectToActionResult("Index", "Auth", null);
                return;
            }

            // Get user from session
            var user = sessionService.GetUserSession<Member>();

            if (user == null)
            {
                context.Result = new RedirectToActionResult("Index", "Auth", null);
                return;
            }

            // Get permissions based on user role
            var permissions = PermissionHelper.GetPermissions(user.UserRole);

            if (!permissions.Contains(_permission))
            {
                context.Result = new RedirectToActionResult(
                    "UnauthorizedAccess",
                    "Auth",
                    new { area = "" }
                );
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}