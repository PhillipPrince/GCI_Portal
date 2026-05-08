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

            var user = sessionService?.GetUserSession<Member>();

            // Not logged in
            if (user == null)
            {
                context.Result = new RedirectToActionResult("Index", "Auth", null);
                return;
            }

            var permissions = PermissionHelper.GetPermissions(user.UserRole);

            if (!permissions.Contains(_permission))
            {
                context.Result = new RedirectToActionResult(
                    "Unauthorized",
                    "Auth",
                    null
                );
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
