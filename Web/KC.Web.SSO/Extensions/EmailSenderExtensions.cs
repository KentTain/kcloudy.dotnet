using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using KC.Web.SSO.Services;
using KC.IdentityServer4.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using KC.IdentityServer4.Models;
using KC.IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using KC.IdentityServer4.Extensions;

namespace KC.Web.SSO.Services
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "Confirm your email",
                $"Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>");
        }
    }

    public static class HttpContextExtensions
    {
        internal static async Task<string> GetIdentityServerSignoutFrameCallbackUrlAsync(this HttpContext context, LogoutMessage logoutMessage = null)
        {
            var userSession = context.RequestServices.GetRequiredService<IUserSession>();
            var user = await userSession.GetUserAsync();
            var currentSubId = user?.GetSubjectId();

            EndSession endSessionMsg = null;

            // if we have a logout message, then that take precedence over the current user
            if (logoutMessage?.ClientIds?.Any() == true)
            {
                var clientIds = logoutMessage?.ClientIds;

                // check if current user is same, since we migth have new clients (albeit unlikely)
                if (currentSubId == logoutMessage?.SubjectId)
                {
                    clientIds = clientIds.Union(await userSession.GetClientListAsync());
                    clientIds = clientIds.Distinct();
                }

                endSessionMsg = new EndSession
                {
                    SubjectId = logoutMessage.SubjectId,
                    SessionId = logoutMessage.SessionId,
                    ClientIds = clientIds
                };
            }
            else if (currentSubId != null)
            {
                // see if current user has any clients they need to signout of 
                var clientIds = await userSession.GetClientListAsync();
                if (clientIds.Any())
                {
                    endSessionMsg = new EndSession
                    {
                        SubjectId = currentSubId,
                        SessionId = await userSession.GetSessionIdAsync(),
                        ClientIds = clientIds
                    };
                }
            }

            if (endSessionMsg != null)
            {
                var clock = context.RequestServices.GetRequiredService<ISystemClock>();
                var msg = new Message<EndSession>(endSessionMsg, clock.UtcNow.UtcDateTime);

                var endSessionMessageStore = context.RequestServices.GetRequiredService<IMessageStore<EndSession>>();
                var id = await endSessionMessageStore.WriteAsync(msg);

                var signoutIframeUrl = context.GetIdentityServerBaseUrl().EnsureTrailingSlash() + RouteEndSessionCallback;
                signoutIframeUrl = signoutIframeUrl.AddQueryString(EndSessionCallback, id);

                return signoutIframeUrl;
            }

            // no sessions, so nothing to cleanup
            return null;
        }
        public const string EndSessionCallback = "endSessionId";
        public const string EndSession = "connect/endsession";
        public const string RouteEndSessionCallback = EndSession + "/callback";

        /// <summary>
        /// Gets the host name of IdentityServer.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static string GetIdentityServerHost(this HttpContext context)
        {
            var request = context.Request;
            return request.Scheme + "://" + request.Host.ToUriComponent();
        }

        /// <summary>
        /// Gets the public base URL for IdentityServer.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static string GetIdentityServerBaseUrl(this HttpContext context)
        {
            return context.GetIdentityServerHost() + context.GetIdentityServerBasePath();
        }

        public static string EnsureTrailingSlash(this string url)
        {
            if (!url.EndsWith("/"))
            {
                return url + "/";
            }

            return url;
        }
        public static string AddQueryString(this string url, string query)
        {
            if (!url.Contains("?"))
            {
                url += "?";
            }
            else if (!url.EndsWith("&"))
            {
                url += "&";
            }

            return url + query;
        }

        public static string AddQueryString(this string url, string name, string value)
        {
            return url.AddQueryString(name + "=" + UrlEncoder.Default.Encode(value));
        }
    }

    

    internal class EndSession
    {
        public string SubjectId { get; set; }
        public string SessionId { get; set; }
        public IEnumerable<string> ClientIds { get; set; }
    }
}
