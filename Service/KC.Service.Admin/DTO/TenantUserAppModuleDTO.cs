using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Service.DTO;

namespace KC.Service.DTO.Admin
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class TenantUserAppModuleDTO : EntityBaseDTO
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public Guid ModuleId { get; set; }
        [DataMember]
        [Display(Name = "模块名称")]
        public string ModuleName { get; set; }
        [DataMember]
        [Display(Name = "模块描述")]
        public string Description { get; set; }
        [DataMember]
        [Display(Name = "模块涉及程序集")]
        public string AssemblyName { get; set; }
        [DataMember]
        public int ApplicationId { get; set; }
        [DataMember]
        public string ApplicationName { get; set; }
        [DataMember]
        public virtual TenantUserApplicationDTO Application { get; set; }
    }
}
