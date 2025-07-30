using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Pay.Constants
{
    public class CPCNReturnCodeConstants
    {
        /// <summary>
        /// 交易成功
        /// </summary>
        public const string Success = "000000";

        /// <summary>
        /// 系统不支持 GET 方法
        /// </summary>
        public const string RQUA01 = "RQUA01";

        /// <summary>
        /// 业务办理失败     
        /// 系统出现异常，需联系客服人员。
        /// </summary>
        public const string SDER01 = "SDER01";

        /// <summary>
        /// 远程通信失败
        /// </summary>
        public const string SDER02 = "SDER02";

        /// <summary>
        /// 业务办理超时
        /// </summary>
        public const string SDER03 = "SDER03";

        /// <summary>
        /// 参数错误
        /// </summary>
        public const string SDER04 = "SDER04";

        /// <summary>
        /// 合作方编号错误
        /// </summary>
        public const string SDER05 = "SDER05";

        /// <summary>
        /// 验证签名失败
        /// </summary>
        public const string SDER06 = "SDER06";

        /// <summary>
        /// 合作方不支持该交易
        /// </summary>
        public const string SDER07 = "SDER07";

        /// <summary>
        /// 数据存储失败
        /// 在保存、更新数据时，编辑条数不符合预期目标，则返回该错误码。
        /// </summary>
        public const string SDER08 = "SDER08";

        /// <summary>
        /// 报文格式错误 [具体错误原因]
        /// 在进行 XML 报文数据格式校验时，如果不符合标准，则返回该错误码。
        /// </summary>
        public const string NW8532 = "NW8532";

        /// <summary>
        /// 查询不到 XXXX 数据
        /// </summary>
        public const string NW8574 = "NW8574";

        /// <summary>
        /// 交易日期错误
        /// 交易日期必须是当前自然日
        /// </summary>
        public const string NW8591 = "NW8591";

        /// <summary>
        /// 账户状态异常[账户状态]
        /// 在账户状态为【未开户】、【已销户】、【已冻结】的情况下返回该错误码。
        /// </summary>
        public const string DB3111 = "DB3111";

        /// <summary>
        /// 账户名称不正确
        /// </summary>
        public const string DB3123 = "DB3123";

        /// <summary>
        /// 单笔转账金额超限
        /// 当 本次转出金额>单笔转账限额时，返回该错误码。当单笔转账限额等于 0 时，不做校验。
        /// </summary>
        public const string DB3135 = "DB3135";

        /// <summary>
        /// 当本次转出金额+当日已成功转出金额 > 每日转账限额 时，返回该错误码。当每日转账限额等于 0 时，不做校验
        /// </summary>
        public const string DB3136 = "DB3136";

        /// <summary>
        /// 账户可用资金不足
        /// 当本次转出金额(含手续费)>账户可用资金 时，返回该错误码。
        /// </summary>
        public const string DB3139 = "DB3139";

        /// <summary>
        /// 账户可解冻资金不足
        /// 当 本次解冻金额>账户冻结资金时，返回该错误码。
        /// </summary>
        public const string DB3155 = "DB3155";

        /// <summary>
        /// 交易金额错误
        /// 当交易金额小于等于 0 时返回该错误码。
        /// </summary>
        public const string DB3151 = "DB3151";

        /// <summary>
        /// 合作方交易流水号重复使用
        /// </summary>
        public const string DB3192 = "DB3192";

        #region T1001 开销户 特有返回码

        /// <summary>
        /// 不能重复开户
        /// </summary>
        public const string SDER09 = "SDER09";

        /// <summary>
        /// 账户中存在资金
        /// 在销户时，如果账户中资金余额大于 0，则返回该错误码。
        /// </summary>
        public const string DB3112 = "DB3112";

        /// <summary>
        /// 最近三天存在交易
        /// 在销户时，如果如果最近三内内存有交易，则返回该错误码。
        /// </summary>
        public const string DB3113 = "DB3113";

        #endregion


        #region T1004 

        /// <summary>
        /// 合作方不支持该业务
        /// 在绑定银行结算账户(银行卡)时，如果合作方不支持该银行绑定提现卡，则返回该错误码。
        /// </summary>
        public const string SDER11 = "SDER11";

        /// <summary>
        /// 已绑定结算账户(银行卡)
        /// 在绑定银行结算账户(银行卡)时，如果该账户已经绑定银行结算账户(银行卡)，则返回该错误码。
        /// </summary>
        public const string DB3120 = "DB3120";

        /// <summary>
        /// 其他客户已经绑定该结算账户
        /// 绑定一个其他客户(身份证号或组织机构代码相同，视为同一个客户)已经绑定的银行结算账户(银行卡)，返回该错误码。
        /// </summary>
        public const string DB3127 = "DB3127";

        #endregion

        /// <summary>
        /// 该交易已经处理成功
        /// 针对同一合作方交易流水号再次发起交易，如果之前发的交易已经处理成功，则返回该错误码，并返回原交易的平台交易流水号
        /// </summary>
        public const string PYSUCC = "PYSUCC";

        /// <summary>
        /// 支付单号重复使用
        /// </summary>
        public const string DB3171 = "DB3171";
    }
}
