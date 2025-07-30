using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.InAndOutTransaction.TransferVoucher
{
    public class MSG
    {
        
        public MSGHD MSGHD { get; set; }

        
        public CltAcc CltAcc { get; set; }

        /// <summary>
        /// 起始日期 YYYYMMDDBkCd=LFYH0001 必传
        /// </summary>
        
        public string QuyStDt { get; set; }

        /// <summary>
        /// 结束日期 YYYYMMDD BkCd=LFYH0001 必传 
        /// </summary>
        
        public string QuyEndDt { get; set; }

        /// <summary>
        /// 查询起始笔数，从 1 开始 R
        /// </summary>
        
        public int StartNum { get; set; }

        /// <summary>
        /// 查询笔数，至多 99 笔 R
        /// </summary>
        
        public int QueryNum { get; set; }

        /// <summary>
        ///  原合作方交易流水号
        /// </summary>
        
        public string QPtnSrl { get; set; }

        /// <summary>
        /// 原平台交易流水号
        /// </summary>
        
        public string QPlatSrl { get; set; }
    }
}
