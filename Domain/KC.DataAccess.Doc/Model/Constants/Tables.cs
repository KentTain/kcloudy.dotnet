using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Model.Doc.Constants
{
    public sealed class Tables
    {
        private const string Prx = "doc_";

        public const string DocTemplate = Prx + "DocTemplate";
        public const string DocTemplateLog = Prx + "DocTemplateLog";
        public const string DocCategory = Prx + "DocCategory";
        public const string DocumentInfo = Prx + "DocumentInfo";
        public const string DocumentLog = Prx + "DocumentLog";
        public const string DocBackup = Prx + "DocBackup";

    }
}
