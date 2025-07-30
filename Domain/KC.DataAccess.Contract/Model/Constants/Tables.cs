using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Model.Contract.Constants
{
    public sealed class Tables
    {
        private const string Prx = "econ_";

        public const string ContractGroup = Prx + "ContractGroup";
        public const string ContractGroupOperationLog = Prx + "ContractGroupOperationLog";
        public const string ContractTemplate = Prx + "ContractTemplate";

        public const string UserContract = Prx + "UserContract";

        public const string ElectronicOrganization = Prx + "ElectronicOrganization";
        public const string ElectronicPerson = Prx + "ElectronicPerson";
        

    }
}
