namespace AiAgent.API.Service
{
    public class HttpService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// This method should only be used in the meantime because you can't identify multiple hosts 
        /// on the same LAN since they share the same public IP.  
        /// The proper way to handle this is to send a cookie to the client,  
        /// and when the client sends a request, the cookie can be added to the packet header. So the server can identify a 
        /// client properly.
        /// <br></br>
        /// response.Headers.AddCookies(new CookieHeaderValue[] {
        ///     new CookieHeaderValue(SessionIdToken, sessionId)
        /// });
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string GetClientIpAddress()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null)
            {
                throw new Exception("Http context not found");
            }

            // Check for the X-Forwarded-For header
            if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                return context.Request.Headers["X-Forwarded-For"].ToString();
            }

            if (context.Connection.RemoteIpAddress is null) throw new Exception("Remote Ip address is null");

            // Fall back to RemoteIpAddress
            return context.Connection.RemoteIpAddress!.ToString();
        }
    }
}
