using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KC.WebApi.Pay.Models.CMB
{
    public class ERPAYSTAZ : CMBBaseModel
    {
        /// <summary>
        /// 业务流水号
        /// </summary>
        public string BUSNBR { get; set; }

        /// <summary>
        /// 客户参考业务号 ERP系统唯一编号
        /// </summary>
        public string REFNBR { get; set; }

        /// <summary>
        /// 错误码:0000000表示成功，否则表示失败
        /// </summary>
        public string ERRCOD { get; set; }

        /// <summary>
        /// 记录状态 0查无此记录（状态可疑） 1：支付成功 2：支付失败 3：未完成 4：银行退票
        /// </summary>
        public string STATUS { get; set; }

        /// <summary>
        /// 业务状态
        /// AC-中心手工录入后需人工复核
        /// AD-待预处理
        /// AE-审批否决
        /// AF-确认支付失败
        /// AM-待审批
        /// AN-待支付重发审批
        /// AQ-业务撤消
        /// AW-待企业审批
        /// BE-支付信息有误
        /// BF-银行支付失败
        /// BP-直联支付中
        /// BQ-状态可疑
        /// BR-银行退票
        /// BT-已提交直联接口进行支付
        /// BW-待银行答复
        /// CC-客户处理中
        /// CD-客户撤消
        /// CF-客户审批否决
        /// CL-已取消支付
        /// CP-待中心处理
        /// DB-部分成功
        /// DP-已支付
        /// EP-待银保通支付
        /// ET-银保通支付中
        /// GC-分公司取消
        /// GD-部分成功
        /// GF-文件导出
        /// GQ-中意可疑状态
        /// MC-待人工确认
        /// OD-过期
        /// PP-待票据打印
        /// RW-等待刷新
        /// </summary>
        public string OPTSTU { get; set; }

        /// <summary>
        /// 备注信息
        /// 支付失败或否决时的错误信息
        /// </summary>
        public string REMARK { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        public string OPRTYP { get; set; }

        /// <summary>
        /// 付款帐号
        /// </summary>
        public string CLTACC { get; set; }
    }
}