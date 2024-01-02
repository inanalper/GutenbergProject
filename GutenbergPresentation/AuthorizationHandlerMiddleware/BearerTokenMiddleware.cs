using Microsoft.AspNetCore.Authentication;

namespace GutenbergPresentation.AuthorizationHandlerMiddleware
{
    public class BearerTokenMiddleware
    {
        private readonly RequestDelegate _next;
      

        public BearerTokenMiddleware(RequestDelegate next)
        {
            _next = next;
           
        }
        public async Task Invoke(HttpContext context)
        {
            var authenticationCookie = context.Request.Cookies["Auth"];

            if (!string.IsNullOrEmpty(authenticationCookie))
            {
                context.Request.Headers.Add("Authorization", $"Bearer {authenticationCookie}");
                
            }

            await _next(context);
        }



    }
}
