using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Model.Blog.Constants
{
    public sealed class Tables
    {
        private const string Prx = "blog_";

        public const string Sequence = Prx + "Sequence";

        public const string Category = Prx + "Category";
        public const string Blog = Prx + "Blog";
        public const string Comment = Prx + "Comment";
        public const string Setting = Prx + "Setting";
    }
}
