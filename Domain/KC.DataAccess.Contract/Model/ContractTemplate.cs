using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Enums.Contract;
using KC.Framework.Base;
using KC.Model.Contract.Constants;

namespace KC.Model.Contract
{
    [Table(Tables.ContractTemplate)]
    public class ContractTemplate : Entity
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// 合同模板名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 合同模板类型
        /// </summary>
        [DataMember]
        public ContractType Type { get; set; }

        /// <summary>
        /// 交易类型
        /// </summary>
        public string TransactionTypeName { get; set; }

        /// <summary>
        /// 合同模板内容
        /// </summary>
        public string ContractValue { get; set; }

        /// <summary>
        /// 合同标题，暂不使用
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 合同内容，客户自己编辑的固定内容
        /// </summary>
        public string Content { get; set; }

    }
}
