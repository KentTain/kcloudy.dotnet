using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.InAndOutTransaction.TransferVoucher.ReturnResponse
{
    public class MSG
    {
        
        public MSGHD MSGHD { get; set; }

        
        public List<QuyDa> QuyDa { get; set; }
    }
}
