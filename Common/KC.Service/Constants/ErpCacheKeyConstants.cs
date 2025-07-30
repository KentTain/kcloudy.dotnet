using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Constants
{
    public class ErpCacheKeyConstants
    {
        private const string ComPrx = "com-";
        private const string ErpPrx = "erp-";
        private const string ArapPrx = "arap-";
        public class Prefix
        {
            public const string MaGroupTree = ComPrx + "MaGroupTree-";

            public const string ErpSetViewPrx = ErpPrx + "SetView-";

            public const string GeneralLedgerTree = ComPrx + "GeneralLedgerTree-";

            public const string GlInitTree = ComPrx + "GlInitTree-";

            public const string CashItemsTree = ComPrx + "CashItemsTree-";

            public const string ArapSetViewPrx = ArapPrx + "SetView-";


        }
    }
}
