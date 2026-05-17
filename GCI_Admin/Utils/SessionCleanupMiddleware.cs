using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace GCI_Admin.Utils
{
    public class SessionCleanupMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SessionCleanupMiddleware> _logger;

        public SessionCleanupMiddleware(RequestDelegate next, ILogger<SessionCleanupMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // List of paths that are allowed without authentication
            var allowedPaths = new[]
            {
                "/Auth/Index",
                "/Auth/Login",
                "/Auth/ConfirmOtp",
                "/Auth/ResendOtp",
                "/Auth/ForgotPassword",
                "/Auth/RequestPasswordReset",
                "/Auth/ResetPassword",
                "/Auth/Unauthorized",
                "/Auth/Logout",
                "/Auth/LogoutGet",
                "/Auth/CheckAuthentication"
            };

            var currentPath = context.Request.Path.ToString();
            var isAllowedPath = false;

            foreach (var path in allowedPaths)
            {
                if (currentPath.Equals(path, System.StringComparison.OrdinalIgnoreCase))
                {
                    isAllowedPath = true;
                    break;
                }
            }

            // If user is authenticated but trying to access login page or unauthorized area
            if (context.User.Identity.IsAuthenticated &&
                (currentPath.Contains("/Auth/Index") ||
                 currentPath.Contains("/Auth/Login") ||
                 currentPath.Contains("/Unauthorized")))
            {
                _logger.LogWarning($"User {context.User.Identity.Name} redirected to wrong page: {currentPath}. Clearing session.");

                // Clear all cookies and session
                context.Response.Cookies.Delete("GCI_Token");
                context.Response.Cookies.Delete(".AspNetCore.Session");
                context.Response.Cookies.Delete("GCI_Auth_Cookie");
                context.Response.Cookies.Delete(".AspNetCore.Cookies");
                context.Session.Clear();
                await context.SignOutAsync();
            }

            await _next(context);
        }
    }
}