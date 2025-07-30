using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Model.CodeGenerate.Constants
{
    public sealed class Tables
    {
        private const string Prx = "code_";

        public const string ModelCategory = Prx + "ModelCategory";
        public const string ModelChangeLog = Prx + "ModelChangeLog";

        public const string ModelDefinition = Prx + "ModelDefinition";
        public const string ModelDefField = Prx + "ModelDefField";

        public const string RelationDefinition = Prx + "RelationDefinition";
        public const string RelationDefDetail = Prx + "RelationDefDetail";

        public const string ApiDefinition = Prx + "ApiDefinition";
        public const string ApiInputParam = Prx + "ApiInputParam";
        public const string ApiOutParam = Prx + "ApiOutParam";

    }
}
