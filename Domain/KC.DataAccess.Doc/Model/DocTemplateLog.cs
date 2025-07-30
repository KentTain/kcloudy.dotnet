using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Model.Doc.Constants;
using KC.Framework.Base;
using KC.Enums.Doc;

namespace KC.Model.Doc
{
    [Table(Tables.DocumentLog)]
    public class DocTemplateLog : ProcessLogBase
    {
        public DocOperType DocOperType { get; set; }

        public int DocTemplateId { get; set; }

        [ForeignKey("DocTemplateId")]
        public DocTemplate DocumentInfo { get; set; }
    }
}
