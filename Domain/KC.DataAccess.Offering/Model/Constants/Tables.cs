using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Model.Offering.Constants
{
    public sealed class Tables
    {
        private const string Prx = "prd_";

        public const string Category = Prx + "Category";
        public const string CategoryManager = Prx + "CategoryManager";
        public const string PropertyProvider = Prx + "PropertyProvider";
        public const string PropertyProviderAttr = Prx + "PropertyProviderAttr";
        public const string CategoryOperationLog = Prx + "CategoryOperationLog";


        public const string Offering = Prx + "Offering";
        public const string OfferingProperty = Prx + "OfferingProperty";
        public const string OfferingPropertyAttr = Prx + "OfferingPropertyAttr";
        public const string OfferingOperationLog = Prx + "OfferingOperationLog";

        public const string Product = Prx + "Product";
        public const string ProductProperty = Prx + "ProductProperty";
        public const string ProductPropertyAttr = Prx + "ProductPropertyAttr";

        
        
    }
}
