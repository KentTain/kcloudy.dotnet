using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KC.Web.Util
{
    public static class ClaimUtil
    {
        public static Claim GetCurrentUserClaim(string claimType)
        {
            return GetClaimsFromPrincipal(Thread.CurrentPrincipal, claimType);
        }

        public static Claim GetClaimsFromPrincipal(IPrincipal principal, string claimType)
        {
            if (principal == null)
            {
                throw new ArgumentNullException("principal");
            }

            var claimsPrincipal = principal as ClaimsPrincipal;
            if (claimsPrincipal == null)
            {
                throw new ArgumentException("Cannot convert principal to IClaimsPrincipal.", "principal");
            }

            return GetClaimFromIdentity(claimsPrincipal.Identities.FirstOrDefault(), claimType);
        }

        public static Claim GetClaimFromIdentity(IIdentity identity, string claimType)
        {
            if (identity == null)
            {
                throw new ArgumentNullException("identity");
            }

            var claimsIdentity = identity as ClaimsIdentity;
            if (claimsIdentity == null)
            {
                throw new ArgumentException("Cannot convert identity to IClaimsIdentity", "identity");
            }

            return claimsIdentity.Claims.SingleOrDefault(c => c.Type == claimType);
        }
    }
}
