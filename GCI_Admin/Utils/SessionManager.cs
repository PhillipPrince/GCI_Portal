using System;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Utils
{
    public class SessionManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string UserSessionKey = "LoggedInUser";
        private const string TokenKey = "UserToken";
        private const string UserPermissionsKey = "UserPermissions";

        public SessionManager(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Save user to session
        /// </summary>
        public void SetUserSession(object user)
        {
            try
            {
                var session = _httpContextAccessor.HttpContext?.Session;
                if (session == null) return;

                var userData = JsonConvert.SerializeObject(user);
                session.SetString(UserSessionKey, userData);
            }
            catch (Exception ex)
            {
                // Log exception if needed
                Console.WriteLine($"Error saving user session: {ex.Message}");
            }
        }

        /// <summary>
        /// Get user from session
        /// </summary>
        public T GetUserSession<T>() where T : class
        {
            try
            {
                var session = _httpContextAccessor.HttpContext?.Session;
                if (session == null) return default;

                var userData = session.GetString(UserSessionKey);
                if (string.IsNullOrEmpty(userData)) return default;

                return JsonConvert.DeserializeObject<T>(userData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting user session: {ex.Message}");
                return default;
            }
        }

        /// <summary>
        /// Get user session as object
        /// </summary>
        public object GetUserSession()
        {
            try
            {
                var session = _httpContextAccessor.HttpContext?.Session;
                if (session == null) return null;

                var userData = session.GetString(UserSessionKey);
                if (string.IsNullOrEmpty(userData)) return null;

                return JsonConvert.DeserializeObject(userData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting user session: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Check if user is logged in
        /// </summary>
        public bool IsUserLoggedIn()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null) return false;

            var userData = session.GetString(UserSessionKey);
            return !string.IsNullOrEmpty(userData);
        }

        /// <summary>
        /// Clear user session
        /// </summary>
        public void ClearSession()
        {
            try
            {
                var session = _httpContextAccessor.HttpContext?.Session;
                session?.Remove(UserSessionKey);
                session?.Remove(TokenKey);
                session?.Remove(UserPermissionsKey);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error clearing session: {ex.Message}");
            }
        }

        /// <summary>
        /// Clear entire session
        /// </summary>
        public void ClearAllSession()
        {
            try
            {
                var session = _httpContextAccessor.HttpContext?.Session;
                session?.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error clearing all session: {ex.Message}");
            }
        }

        /// <summary>
        /// Save JWT token to session
        /// </summary>
        public void SetToken(string token)
        {
            try
            {
                var session = _httpContextAccessor.HttpContext?.Session;
                if (session == null) return;

                session.SetString(TokenKey, token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving token: {ex.Message}");
            }
        }

        /// <summary>
        /// Get JWT token from session
        /// </summary>
        public string GetToken()
        {
            try
            {
                var session = _httpContextAccessor.HttpContext?.Session;
                if (session == null) return null;

                return session.GetString(TokenKey);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting token: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Save user permissions to session
        /// </summary>
        public void SetUserPermissions(string[] permissions)
        {
            try
            {
                var session = _httpContextAccessor.HttpContext?.Session;
                if (session == null) return;

                var permissionsData = JsonConvert.SerializeObject(permissions);
                session.SetString(UserPermissionsKey, permissionsData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving permissions: {ex.Message}");
            }
        }

        /// <summary>
        /// Get user permissions from session
        /// </summary>
        public string[] GetUserPermissions()
        {
            try
            {
                var session = _httpContextAccessor.HttpContext?.Session;
                if (session == null) return Array.Empty<string>();

                var permissionsData = session.GetString(UserPermissionsKey);
                if (string.IsNullOrEmpty(permissionsData)) return Array.Empty<string>();

                return JsonConvert.DeserializeObject<string[]>(permissionsData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting permissions: {ex.Message}");
                return Array.Empty<string>();
            }
        }

        /// <summary>
        /// Check if user has specific permission
        /// </summary>
        public bool HasPermission(string permission)
        {
            var permissions = GetUserPermissions();
            return permissions.Contains(permission);
        }

        /// <summary>
        /// Get user role from session (if stored in user object)
        /// </summary>
        public int? GetUserRoleId()
        {
            try
            {
                var user = GetUserSession();
                if (user == null) return null;

                var userType = user.GetType();
                var roleIdProperty = userType.GetProperty("UserRoleId") ?? userType.GetProperty("RoleId");

                if (roleIdProperty != null)
                {
                    return Convert.ToInt32(roleIdProperty.GetValue(user));
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Refresh user session with updated data
        /// </summary>
        public void RefreshUserSession(object updatedUser)
        {
            ClearSession();
            SetUserSession(updatedUser);
        }
    }
}