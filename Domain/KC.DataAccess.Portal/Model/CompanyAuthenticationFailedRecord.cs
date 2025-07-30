using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KC.Model.Portal.Constants;
using KC.Framework.Base;

namespace KC.Model.Portal
{
    /// <summary>
    /// 用户认证失败记录
    /// </summary>
    [Table(Tables.CompanyAuthenticationFailedRecord)]
    public class CompanyAuthenticationFailedRecord : ProcessLogBase
    {
        public CompanyAuthenticationFailedRecord()
        {
            Type = ProcessLogType.Failure;
        }

        /// <summary>
        /// 企业编码：（SequenceName--Organization：ORG2018120100001）
        /// </summary>
        [MaxLength(20)]
        public string CompanyCode { get; set; }
        /// <summary>
        /// 企业名称
        /// </summary>
        [MaxLength(512)]
        public string CompanyName { get; set; }

        /// <summary>
        /// 认证失败字段属性名
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 认证失败字段属性显示名
        /// </summary>
        public string KeyName { get; set; }
    }
}
