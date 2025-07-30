using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Extension;
using KC.Service.DTO;
using KC.Enums.Contract;

namespace KC.Service.DTO.Contract
{
    public class UserContractAPIModel : EntityDTO
    {
        /// <summary>
        /// 子合同主键Id
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }
        /// <summary>
        /// 签署人租户代码
        /// </summary>
        [DataMember]
        public string UserId { get; set; }
        /// <summary>
        /// 签署人企业名称
        /// </summary>
        [DataMember]
        public string UserName { get; set; }
        /// <summary>
        /// 合同签署关键字
        /// </summary>
        [DataMember]
        public string Key { get; set; }
        /// <summary>
        /// 会员类型
        /// </summary>
        [DataMember]
        public CustomerType CustomerType { get; set; }
        /// <summary>
        /// 子合同状态
        /// </summary>
        [DataMember]
        public UserContractStatus Statu { get; set; }
        /// <summary>
        /// 子合同状态字符串格式
        /// </summary>
        [DataMember]
        public string StatuStr
        {
            get
            {
                return Statu.ToDescription();
            }
        }
        /// <summary>
        /// 退回或者作废原因
        /// </summary>
        [DataMember]
        public string BreakRemark { get; set; }
        /// <summary>
        /// 关联主合同Id
        /// </summary>
        [DataMember]
        public Guid BlobId { get; set; }
 
        /// <summary>
        /// 是否可以编辑
        /// </summary>
        [DataMember]
        public bool IsEdit { get; set; }
    }
}
