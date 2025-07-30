using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KC.WebApi.Pay.Models.CMB
{
    public class APPAYSAVX
    {
        /// <summary>
        /// 记录序号
        /// 单笔提交时值为1
        /// 不可空
        /// </summary>
        public int RECNUM { get; set; }

        /// <summary>
        /// 操作类型
        /// 202-对外支付
        /// 204-银行调拨
        /// 206-内转
        /// 401-集中支付
        ///235-网银互联
        ///236-集中网银互联
        ///215-联动支付
        /// 不可空
        /// </summary>
        public string OPRTYP { get; set; }

        /// <summary>
        /// 业务子类型
        /// 0-标准支付
        /// 不可空
        /// </summary>
        public int BUSTYP { get; set; }

        /// <summary>
        /// 客户参考业务号(ERP系统唯一编号，此编号同一渠道不能重复提交)
        /// 不可空
        /// </summary>
        public string REFNBR { get; set; }

        /// <summary>
        /// 期望日
        /// 直连提交银行的付款日期
        /// </summary>
        public string EPTDAT { get; set; }

        /// <summary>
        /// 期望时间
        /// </summary>
        public string EPTTIM { get; set; }

        /// <summary>
        /// 付方账号
        /// 对于内转业务为内部付款账号，其他业务为付款银行账号，账号必须系统内登记且用户有操作权限集中支付时为空
        /// </summary>
        public string CLTACC { get; set; }

        /// <summary>
        /// 付方客户号
        /// C（4）
        /// 付款账号所属客户号
        /// 不可空
        /// </summary>
        public string CLTNBR { get; set; }

        /// <summary>
        /// 金额
        /// M(15,2)
        /// 不可空
        /// </summary>
        public decimal TRSAMT { get; set; }

        /// <summary>
        /// 币种
        /// C（2）
        /// 参照币种参数取值
        /// </summary>
        public string CCYNBR { get; set; }

        /// <summary>
        /// 交易用途
        /// Z(1,60)
        /// 不可空
        /// </summary>
        public string TRSUSE { get; set; }

        /// <summary>
        /// 摘要
        /// Z(1,60)
        /// </summary>
        public string EXTTX1 { get; set; }

        /// <summary>
        /// 内部户账号
        /// 对外支付以及集中支付业务可输入，其他业务为空
        /// C（1,20）
        /// </summary>
        public string INNACC { get; set; }

        /// <summary>
        /// 收款人账号
        /// C（1,35）
        /// 内转时为收款内部户，其他业务为收款银行账号
        /// 不可空
        /// </summary>
        public string REVACC { get; set; }

        /// <summary>
        /// 收款人开户行
        /// Z（1,60）
        /// 对外支付、银行调拨、集中支付不能为空
        /// </summary>
        public string REVBNK { get; set; }

        /// <summary>
        /// 收款人名称
        /// Z（1,60）
        /// 对外支付、银行调拨、集中支付不能为空
        /// </summary>
        public string REVNAM { get; set; }

        /// <summary>
        /// 收方省份
        /// Z（1,20）
        /// 对外支付、银行调拨、集中支付不能为空
        /// </summary>
        public string REVPRV { get; set; }

        /// <summary>
        /// 收方城市
        /// Z（1,30）
        /// 对外支付、银行调拨、集中支付不能为空
        /// </summary>
        public string REVCIT { get; set; }

        /// <summary>
        /// C（1,5）
        /// 收款银行类型
        /// 对外支付、银行调拨、集中支付不能为空
        /// </summary>
        public string BNKTYP { get; set; }

        /// <summary>
        /// C（1,10）
        /// 银行号
        /// </summary>
        public string BNKPRM { get; set; }

        /// <summary>
        /// 银行联行号
        /// </summary>
        public string BRDNBR { get; set; }

        /// <summary>
        /// 支付渠道
        /// 0-其他
        /// 2-打印银行票据
        /// 3-银企直联支付
        /// 5-银保通支付
        /// 付款银行账号开通直连时，默认选择3
        /// </summary>
        public int OPRMOD { get; set; }

        /// <summary>
        /// 结算方式
        /// C（1）
        /// 0-其它
        /// 2-转账
        /// 3-电汇
        /// 4-汇票
        /// </summary>
        public int PAYTYP { get; set; }

        /// <summary>
        /// 公私标志
        /// </summary>
        public int COPFLG { get; set; }

        /// <summary>
        /// 是否同城
        /// 0-	同城1-	异地
        /// </summary>
        public int CTYFLG { get; set; }

        /// <summary>
        /// 是否加急
        /// Y-定向或加急
        /// N-不定向或普通
        /// </summary>
        public string PAYSON { get; set; }

        /// <summary>
        /// 预算项编号
        /// C(1,20)
        /// </summary>
        public string ITMNBR { get; set; }

        /// <summary>
        /// 资金计划流水号
        /// C(9)
        /// </summary>
        public string PLNNBR { get; set; }

        /// <summary>
        /// ERP备注1
        /// </summary>
        public string ERPCM1 { get; set; }

        /// <summary>
        /// 付款总账号
        /// C(1,35)
        /// </summary>
        public string REVCAT { get; set; }

        /// <summary>
        /// 支付类别
        /// </summary>
        public string PAYCAT { get; set; }
    }
}
