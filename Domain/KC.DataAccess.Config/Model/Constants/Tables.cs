using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Model.Config.Constants
{
    public sealed class Tables
    {
        private const string Prx = "cfg_";

        public const string SysSequence = Prx + "SysSequence";
        public const string SeedEntity = Prx + "SeedEntity";

        public const string ConfigEntity = Prx + "ConfigEntity";
        public const string ConfigAttribute = Prx + "ConfigAttribute";
        public const string ConfigLog = Prx + "ConfigLog";
    }
}
