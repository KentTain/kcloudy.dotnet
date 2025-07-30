using KC.Framework.Util;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KC.Web.Extension
{
    //fixed the cookie is too large: https://www.daimto.com/headersize-issue-with-IdentityServer4/
    public class MemoryCacheTicketStore : ITicketStore
    {
        private readonly int _expireInMinutes = 1;
        private const string KeyPrefix = "AuthSessionStore-";
        private IMemoryCache _cache;

        public MemoryCacheTicketStore(int catchExpiration)
        {
            var cacheOption = new MemoryCacheOptions();
            _cache = new MemoryCache(cacheOption);
            _expireInMinutes = catchExpiration;
        }

        public async Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var key = $"{KeyPrefix}{Guid.NewGuid()}";
            await RenewAsync(key, ticket);
            return key;
        }

        public Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            var options = new MemoryCacheEntryOptions();
            var expiresUtc = ticket.Properties.ExpiresUtc;
            if (expiresUtc.HasValue)
            {
                options.SetAbsoluteExpiration(expiresUtc.Value);
            }
            else
            {
                options.SetAbsoluteExpiration(TimeSpan.FromMinutes(_expireInMinutes));
            }

            LogUtil.LogDebug("MemoryCacheTicketStore expiresUtc: " + expiresUtc);

            //options.SetSlidingExpiration(TimeSpan.FromMinutes(_expireInMinutes));

            _cache.Set(key, ticket, options);

            return Task.FromResult(0);
        }

        public Task<AuthenticationTicket> RetrieveAsync(string key)
        {
            _cache.TryGetValue(key, out AuthenticationTicket ticket);
            return Task.FromResult(ticket);
        }

        public Task RemoveAsync(string key)
        {
            _cache.Remove(key);
            return Task.FromResult(0);
        }
    }
}
