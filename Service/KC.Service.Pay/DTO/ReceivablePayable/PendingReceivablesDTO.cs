using KC.Enums.Pay;
using KC.Framework.Extension;
using KC.Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.DTO.Pay
{
    public class PendingReceivablesDTO : EntityBaseDTO
    {
        public string Id { get; set; }

        /// <summary>
        /// 支付编号,系统生成
        /// </summary>
        public string PaymentNumber { get; set; }

        public ReceivableSource Source { get; set; }

        public string SourceName { get { return Source.ToDescription(); } }

        public string OrderId { get; set; }

        public string Customer { get; set; }

        public DateTime CreateDateTime { get; set; }

        public decimal ThisAmounts { get; set; }

        public PaymentType PaymentType { get; set; }

        public string BillNo { get; set; }

        public string FileUrl { get; set; }

        public string Remark { get; set; }

        public string TypeName { get { return PaymentType.ToDescription(); } }

        public string CreateDateTimeStr { get { return CreateDateTime.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss"); } }

        public bool CanReturn { get; set; }
    }
}
