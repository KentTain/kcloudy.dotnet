using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Extension;
using KC.Enums.CRM;
using KC.Service.DTO;

namespace KC.Service.DTO.Customer
{
    public class CustomerContactInfoDTO:EntityBaseDTO
    {
        public CustomerContactInfoDTO()
        {
            this.CustomerManagers=new List<CustomerManagerDTO>();
        }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int CustomerContactId { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhoneNumber { get; set; } 
        public string PositionName { get; set; }
        public CompanyType CompanyType { get; set; }
        public string CompanyTypeStr
        {
            get { return CompanyType.ToDescription(); }
        }
        public ClientType ClientType { get; set; }
        public string ClientTypeStr
        {
            get { return ClientType.ToDescription(); }
        }
        public string AffiliatedCompany { get; set; }
        public bool IsDefaultCustomerContact { get; set; }
        
        public List<CustomerManagerDTO> CustomerManagers { get; set; } 
    }
}
