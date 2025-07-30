using KC.Enums.CRM;
using KC.Framework.Extension;
using KC.Service.DTO.Customer;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KC.Web.Customer.Models
{
    public class CustomerContactInfoViewModel
    {
        public int CurrentContact { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int CustomerContactId { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhoneNumber { get; set; }
        public string PositionName { get; set; }
        public CompanyType CustomerType { get; set; }
        public string CustomerTypeStr { get; set; }
        public string AffiliatedCompany { get; set; }
        public bool IsDefaultCustomerContact { get; set; }
        public ClientType ClientType { get; set; }
        public string ClientTypeStr { get; set; }
        public IEnumerable<SelectListItem> CustomerTypeList { get; set; }
        public IEnumerable<SelectListItem> ProcessLogTypeList { get; set; }
        public IEnumerable<SelectListItem> TracingTypeList { get; set; }

        public static CustomerContactInfoViewModel FromDto(CustomerContactInfoDTO model, int currentContact, IEnumerable<SelectListItem> customerTypeList, IEnumerable<SelectListItem> processLogTypeList, IEnumerable<SelectListItem> tracingTypeList = null)
        {
            return new CustomerContactInfoViewModel()
            {
                CurrentContact = currentContact,

                CustomerId = model.CustomerId,
                CustomerName = model.CustomerName,
                CustomerType = model.CompanyType,
                CustomerTypeStr = model.CompanyTypeStr,
                ClientType = model.ClientType,
                ClientTypeStr = model.ClientTypeStr,

                CustomerContactId = model.CustomerContactId,
                ContactName = model.ContactName,
                ContactEmail = model.ContactEmail,
                ContactPhoneNumber = model.ContactPhoneNumber,
                PositionName = model.PositionName,
                AffiliatedCompany = model.AffiliatedCompany,
                IsDefaultCustomerContact = model.IsDefaultCustomerContact,

                CustomerTypeList = customerTypeList,
                ProcessLogTypeList = processLogTypeList,
                TracingTypeList = tracingTypeList,
            };
        }
    }
}
