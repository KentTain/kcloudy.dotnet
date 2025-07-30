using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;
using KC.Model.Supplier.Constants;

namespace KC.Model.Supplier
{
    /// <summary>
    /// 国民经济行业分类
    /// </summary>
    [Table(Tables.IndustryClassfication)]
    public class IndustryClassfication : TreeNode<IndustryClassfication>
    {
        [DefaultValue(true)]
        public bool IsValid { get; set; }
    }
}
