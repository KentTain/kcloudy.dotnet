using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Extension;
using KC.Enums.CRM;
using KC.Service.DTO;

namespace KC.Service.DTO.Customer
{
    public class CustomerSeasInfoDTO : EntityBaseDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public CompanyType CustomerType { get; set; }
        public string CustomerTypeStr
        {
            get { return CustomerType.ToDescription(); }
        }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhoneNumber { get; set; }
        public string OperatorId { get; set; }
        public string Operator { get; set; }
        public int? OrganizationId { get; set; }
        public DateTime? OperateDate { get; set; }
        public string LocalOperateDate
        {
            get
            {
                return OperateDate.HasValue ? OperateDate.Value.ToLocalDateTime().ToString("yyyy-MM-dd") : String.Empty;
            }
        }
    }
}
