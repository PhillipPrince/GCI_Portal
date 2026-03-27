using System;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Utils
{
    public class SessionManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string UserSessionKey = "LoggedInUser";

        public SessionManager(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Save user to session
        /// </summary>
        public void SetUserSession(object user)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null) return;

            var userData = JsonConvert.SerializeObject(user);
            session.SetString(UserSessionKey, userData);
        }

        /// <summary>
        /// Get user from session
        /// </summary>
        public T GetUserSession<T>()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null) return default;

            var userData = session.GetString(UserSessionKey);
            return userData == null ? default : JsonConvert.DeserializeObject<T>(userData);
        }

        /// <summary>
        /// Clear user session
        /// </summary>
        public void ClearSession()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            session?.Remove(UserSessionKey);
        }
    }
}