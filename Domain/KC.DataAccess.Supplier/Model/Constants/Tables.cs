using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Model.Supplier.Constants
{
    public sealed class Tables
    {
        private const string Prx = "dic_";

        public const string City = Prx + "City";
        public const string Province = Prx + "Province";
        public const string MobileLocation = Prx + "MobileLocation";
        public const string IndustryClassfication = Prx + "IndustryClassfication";

        public const string UserLogin = Prx + "UserLogin";
        public const string UserClaim = Prx + "UserClaim";
        public const string UserToken = Prx + "UserToken";

        
    }
}
