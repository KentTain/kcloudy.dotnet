using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace KC.Web.Extension
{
    public class SecurityHeadersAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            var result = context.Result;
            if (result is ViewResult)
            {
                if (!context.HttpContext.Response.Headers.ContainsKey("X-Content-Type-Options"))
                {
                    context.HttpContext.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                }
                if (!context.HttpContext.Response.Headers.ContainsKey("X-Frame-Options"))
                {
                    context.HttpContext.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
                }
                if (!context.HttpContext.Response.Headers.ContainsKey("Access-Control-Allow-Origin"))
                {
                    context.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                }

                // an example if you need client images to be displayed from twitter
                //var csp = "default-src 'self'; img-src 'self' https://pbs.twimg.com";
                var csp = "default-src 'self' *.kcloudy.com *.docapi.kcloudy.com localhost:9999 'unsafe-eval' 'unsafe-inline'; img-src *; media-src *;font-src *;";
                
                // once for standards compliant browsers
                if (!context.HttpContext.Response.Headers.ContainsKey("Content-Security-Policy"))
                {
                    context.HttpContext.Response.Headers.Add("Content-Security-Policy", csp);
                }
                // and once again for IE
                if (!context.HttpContext.Response.Headers.ContainsKey("X-Content-Security-Policy"))
                {
                    context.HttpContext.Response.Headers.Add("X-Content-Security-Policy", csp);
                }
            }
        }
    }
}
