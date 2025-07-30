using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace KC.WebApi.Pay.Models.AccountTransaction.OpenAccount
{
    /// <summary>
    /// 客户信息
    /// </summary>
    public class Clt
    {
        /// <summary>
        /// 客户性质(0 个人;1 公司) 必填
        /// </summary>
        
        public int Kd { get; set; }

        /// <summary>
        /// 姓名(当 Clt. Kd=0 时，该节点的内容必须与节点CltAcc.CltNm 的内容一致)公司：法定代表人个人：自然人
        /// 必填  长度128
        /// </summary>
        
        public string Nm { get; set; }

        /// <summary>
        /// 法定代表人/自然人证件类型 必填 数字字典
        /// </summary>
        
        public string CdTp { get; set; }

        /// <summary>
        /// 法定代表人/自然人证件号码 必填 长度32
        /// </summary>
        
        public string CdNo { get; set; }


        /// <summary>
        /// 统一社会信用代码(公司必填)填该字段时， 下面 3 个字段可不填值 长度：32
        /// </summary>
        
        public string UscId { get; set; }
        /// <summary>
        /// 组织机构代码证(公司必填) UscId 有值时可不填该字段 长度：32
        /// </summary>
        
        public string OrgCd { get; set; }
        /// <summary>
        /// 营业执照(公司必填) UscId 有值时可不填该字段 长度：32
        /// </summary>
        
        public string BsLic { get; set; }
        /// <summary>
        /// 税务登记号(公司必填)UscId 有值时可不填该字段 长度：32
        /// </summary>
        
        public string Swdjh { get; set; }

        /// <summary>
        /// 手机号码  必填 长度：24 （BkCd=HXYH0001 且个人开户时，手机号要填成个人银行卡的预留手机号，否则无法绑卡（T1004））
        /// </summary>
        
        public string MobNo { get; set; }

        /// <summary>
        /// 电子邮箱 BkCd=HXYH0001 时必填
        /// </summary>
        
        public string Email { get; set; }

        /// <summary>
        /// 邮政编码 长度6 选填
        /// </summary>
        
        public string PostNo { get; set; }

        /// <summary>
        /// 地址长度128 选填
        /// </summary>
        
        public string Addr { get; set; }

    }
}