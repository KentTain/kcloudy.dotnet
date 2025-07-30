using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.InAndOutTransaction.PrintTransferVoucher.ReturnResponse
{
    public class MSG
    {
        
        public MSGHD MSGHD { get; set; }

        /// <summary>
        /// PDF 文件 URL
        /// </summary>
        
        public string PdfFile { get; set; }
    }
}
