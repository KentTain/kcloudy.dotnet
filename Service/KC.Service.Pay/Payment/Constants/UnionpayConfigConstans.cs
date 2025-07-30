using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Pay.Constants
{
    public class UnionpayConfigConstans
    {
        public const string PaymentMethod = "PaymentMethod";

        public const string COLUMN_DEFAULT = "-";

        public const string KEY_DELIMITER = "#";

        /** memeber variable: blank. */
        public const string BLANK = "";

        /** member variabel: space. */
        public const string SPACE = " ";

        /** memeber variable: unline. */
        public const string UNLINE = "_";

        /** memeber varibale: star. */
        public const string STAR = "*";

        /** memeber variable: line. */
        public const string LINE = "-";

        /** memeber variable: add. */
        public const string ADD = "+";

        /** memeber variable: colon. */
        public const string COLON = "|";

        /** memeber variable: point. */
        public const string POINT = ".";

        /** memeber variable: comma. */
        public const string COMMA = ",";

        /** memeber variable: slash. */
        public const string SLASH = "/";

        /** memeber variable: div. */
        public const string DIV = "/";

        /** memeber variable: left . */
        public const string LB = "(";

        /** memeber variable: right. */
        public const string RB = ")";

        /** memeber variable: rmb. */
        public const string CUR_RMB = "RMB";

        /** memeber variable: .page size */
        public const int PAGE_SIZE = 10;

        /** memeber variable: string ONE. */
        public const string ONE = "1";

        /** memeber variable: string ZERO. */
        public const string ZERO = "0";

        /** memeber variable: number six. */
        public const int NUM_SIX = 6;

        /** memeber variable: equal mark. */
        public const string EQUAL = "=";

        /** memeber variable: operation ne. */
        public const string NE = "!=";

        /** memeber variable: operation le. */
        public const string LE = "<=";

        /** memeber variable: operation ge. */
        public const string GE = ">=";

        /** memeber variable: operation lt. */
        public const string LT = "<";

        /** memeber variable: operation gt. */
        public const string GT = ">";

        /** memeber variable: list separator. */
        public const string SEP = "./";

        /** memeber variable: Y. */
        public const string Y = "Y";

        /** memeber variable: AMPERSAND. */
        public const string AMPERSAND = "&";

        /** memeber variable: SQL_LIKE_TAG. */
        public const string SQL_LIKE_TAG = "%";

        /** memeber variable: @. */
        public const string MAIL = "@";

        /** memeber variable: number zero. */
        public const int NZERO = 0;

        public const string LEFT_BRACE = "{";

        public const string RIGHT_BRACE = "}";

        /** memeber variable: string true. */
        public const string TRUE_string = "true";
        /** memeber variable: string false. */
        public const string FALSE_string = "false";

        /** memeber variable: forward success. */
        public const string SUCCESS = "success";
        /** memeber variable: forward fail. */
        public const string FAIL = "fail";
        /** memeber variable: global forward success. */
        public const string GLOBAL_SUCCESS = "$success";
        /** memeber variable: global forward fail. */
        public const string GLOBAL_FAIL = "$fail";

        public const string UTF_8_ENCODING = "UTF-8";
        public const string GBK_ENCODING = "GBK";
        public const string CONTENT_TYPE = "Content-type";
        public const string APP_XML_TYPE = "application/xml;charset=utf-8";
        public const string APP_FORM_TYPE = "application/x-www-form-urlencoded;charset=";

        /******************************************** 5.0报文接口定义 ********************************************/
        /** 版本号. */
        public const string param_version = "version";
        /** 证书ID. */
        public const string param_certId = "certId";
        /** 签名. */
        public const string param_signature = "signature";

        //签名方法
        public const string param_signMethod = "signMethod";

        /** 编码方式. */
        public const string param_encoding = "encoding";
        /** 交易类型. */
        public const string param_txnType = "txnType";
        /** 交易子类. */
        public const string param_txnSubType = "txnSubType";
        /** 业务类型. */
        public const string param_bizType = "bizType";
        /** 前台通知地址 . */
        public const string param_frontUrl = "frontUrl";
        /** 后台通知地址. */
        public const string param_backUrl = "backUrl";
        /** 接入类型. */
        public const string param_accessType = "accessType";
        /** 收单机构代码. */
        public const string param_acqInsCode = "acqInsCode";
        /** 商户类别. */
        public const string param_merCatCode = "merCatCode";
        /** 商户类型. */
        public const string param_merType = "merType";
        /** 商户代码. */
        public const string param_merId = "merId";
        /** 商户名称. */
        public const string param_merName = "merName";
        /** 商户简称. */
        public const string param_merAbbr = "merAbbr";
        /** 二级商户代码. */
        public const string param_subMerId = "subMerId";
        /** 二级商户名称. */
        public const string param_subMerName = "subMerName";
        /** 二级商户简称. */
        public const string param_subMerAbbr = "subMerAbbr";
        /** Cupsecure 商户代码. */
        public const string param_csMerId = "csMerId";
        /** 商户订单号. */
        public const string param_orderId = "orderId";
        /** 交易时间. */
        public const string param_txnTime = "txnTime";
        /** 发送时间. */
        public const string param_txnSendTime = "txnSendTime";
        /** 订单超时时间间隔. */
        public const string param_orderTimeoutInterval = "orderTimeoutInterval";
        /** 支付超时时间. */
        public const string param_payTimeoutTime = "payTimeoutTime";
        /** 默认支付方式. */
        public const string param_defaultPayType = "defaultPayType";
        /** 支持支付方式. */
        public const string param_supPayType = "supPayType";
        /** 支付方式. */
        public const string param_payType = "payType";
        /** 自定义支付方式. */
        public const string param_customPayType = "customPayType";
        /** 物流标识. */
        public const string param_shippingFlag = "shippingFlag";
        /** 收货地址-国家. */
        public const string param_shippingCountryCode = "shippingCountryCode";
        /** 收货地址-省. */
        public const string param_shippingProvinceCode = "shippingProvinceCode";
        /** 收货地址-市. */
        public const string param_shippingCityCode = "shippingCityCode";
        /** 收货地址-地区. */
        public const string param_shippingDistrictCode = "shippingDistrictCode";
        /** 收货地址-详细. */
        public const string param_shippingStreet = "shippingStreet";
        /** 商品总类. */
        public const string param_commodityCategory = "commodityCategory";
        /** 商品名称. */
        public const string param_commodityName = "commodityName";
        /** 商品URL. */
        public const string param_commodityUrl = "commodityUrl";
        /** 商品单价. */
        public const string param_commodityUnitPrice = "commodityUnitPrice";
        /** 商品数量. */
        public const string param_commodityQty = "commodityQty";
        /** 是否预授权. */
        public const string param_isPreAuth = "isPreAuth";
        /** 币种. */
        public const string param_currencyCode = "currencyCode";
        /** 账户类型. */
        public const string param_accType = "accType";
        /** 账号. */
        public const string param_accNo = "accNo";
        /** 支付卡类型. */
        public const string param_payCardType = "payCardType";
        /** 发卡机构代码. */
        public const string param_issInsCode = "issInsCode";
        /** 持卡人信息. */
        public const string param_customerInfo = "customerInfo";
        /** 交易金额. */
        public const string param_txnAmt = "txnAmt";
        /** 余额. */
        public const string param_balance = "balance";
        /** 地区代码. */
        public const string param_districtCode = "districtCode";
        /** 附加地区代码. */
        public const string param_additionalDistrictCode = "additionalDistrictCode";
        /** 账单类型. */
        public const string param_billType = "billType";
        /** 账单号码. */
        public const string param_billNo = "billNo";
        /** 账单月份. */
        public const string param_billMonth = "billMonth";
        /** 账单查询要素. */
        public const string param_billQueryInfo = "billQueryInfo";
        /** 账单详情. */
        public const string param_billDetailInfo = "billDetailInfo";
        /** 账单金额. */
        public const string param_billAmt = "billAmt";
        /** 账单金额符号. */
        public const string param_billAmtSign = "billAmtSign";
        /** 绑定标识号. */
        public const string param_bindId = "bindId";
        /** 风险级别. */
        public const string param_riskLevel = "riskLevel";
        /** 绑定信息条数. */
        public const string param_bindInfoQty = "bindInfoQty";
        /** 绑定信息集. */
        public const string param_bindInfoList = "bindInfoList";
        /** 批次号. */
        public const string param_batchNo = "batchNo";
        /** 总笔数. */
        public const string param_totalQty = "totalQty";
        /** 总金额. */
        public const string param_totalAmt = "totalAmt";
        /** 文件类型. */
        public const string param_fileType = "fileType";
        /** 文件名称. */
        public const string param_fileName = "fileName";
        /** 批量文件内容. */
        public const string param_fileContent = "fileContent";
        /** 商户摘要. */
        public const string param_merNote = "merNote";
        /** 商户自定义域. */
        // public  const string param_merReserved = "merReserved";//接口变更删除
        /** 请求方保留域. */
        public const string param_reqReserved = "reqReserved";// 新增接口
        /** 保留域. */
        public const string param_reserved = "reserved";
        /** 终端号. */
        public const string param_termId = "termId";
        /** 终端类型. */
        public const string param_termType = "termType";
        /** 交互模式. */
        public const string param_interactMode = "interactMode";
        /** 发卡机构识别模式. */
        // public  const string param_recognitionMode = "recognitionMode";
        public const string param_issuerIdentifyMode = "issuerIdentifyMode";// 接口名称变更
        /** 商户端用户号. */
        public const string param_merUserId = "merUserId";
        /** 持卡人IP. */
        public const string param_customerIp = "customerIp";
        /** 查询流水号. */
        public const string param_queryId = "queryId";
        /** 原交易查询流水号. */
        public const string param_origQryId = "origQryId";
        /** 系统跟踪号. */
        public const string param_traceNo = "traceNo";
        /** 交易传输时间. */
        public const string param_traceTime = "traceTime";
        /** 清算日期. */
        public const string param_settleDate = "settleDate";
        /** 清算币种. */
        public const string param_settleCurrencyCode = "settleCurrencyCode";
        /** 清算金额. */
        public const string param_settleAmt = "settleAmt";
        /** 清算汇率. */
        public const string param_exchangeRate = "exchangeRate";
        /** 兑换日期. */
        public const string param_exchangeDate = "exchangeDate";
        /** 响应时间. */
        public const string param_respTime = "respTime";
        /** 原交易应答码. */
        public const string param_origRespCode = "origRespCode";
        /** 原交易应答信息. */
        public const string param_origRespMsg = "origRespMsg";
        /** 应答码. */
        public const string param_respCode = "respCode";
        /** 应答码信息. */
        public const string param_respMsg = "respMsg";
        // 新增四个报文字段merUserRegDt merUserEmail checkFlag activateStatus
        /** 商户端用户注册时间. */
        public const string param_merUserRegDt = "merUserRegDt";
        /** 商户端用户注册邮箱. */
        public const string param_merUserEmail = "merUserEmail";
        /** 验证标识. */
        public const string param_checkFlag = "checkFlag";
        /** 开通状态. */
        public const string param_activateStatus = "activateStatus";
        /** 加密证书ID. */
        public const string param_encryptCertId = "encryptCertId";
        /** 用户MAC、IMEI串号、SSID. */
        public const string param_userMac = "userMac";
        /** 关联交易. */
        // public  const string param_relationTxnType = "relationTxnType";
        /** 短信类型 */
        public const string param_smsType = "smsType";

        /** 风控信息域 */
        public const string param_riskCtrlInfo = "riskCtrlInfo";

        /** IC卡交易信息域 */
        public const string param_ICTransData = "ICTransData";

        /** VPC交易信息域 */
        public const string param_VPCTransData = "VPCTransData";

        /** 安全类型 */
        public const string param_securityType = "securityType";

        /** 银联订单号 */
        public const string param_tn = "tn";

        /** 分期付款手续费率 */
        public const string param_instalRate = "instalRate";

        /** 分期付款手续费率 */
        public const string param_mchntFeeSubsidy = "mchntFeeSubsidy";

        /// <summary>
        /// 渠道类型
        /// </summary>
        public const string param_channelType = "channelType";

        /// <summary>
        /// 有卡交易地址
        /// </summary>
        public const string CardRequestUrl = "CardRequestUrl";

        /// <summary>
        /// app交易地址 手机控件支付使用该地址
        /// </summary>
        public const string AppRequestUrl = "AppRequestUrl";

        /// <summary>
        /// 前台交易地址
        /// </summary>
        public const string FrontTransUrl = "FrontTransUrl";

        /// <summary>
        /// 加密公钥证书名称
        /// </summary>
        public const string EncryptCertName = "EncryptCertName";

        /// <summary>
        /// 后台交易地址
        /// </summary>
        public const string BackTransUrl = "BackTransUrl";

        /// <summary>
        /// 交易状态查询地址
        /// </summary>
        public const string SingleQueryUrl = "SingleQueryUrl";

        /// <summary>
        /// 文件传输类交易地址
        /// </summary>
        public const string FileTransUrl = "FileTransUrl";

        /// <summary>
        /// 签名证书名称
        /// </summary>
        public const string SignCertName = "SignCertName";

        /// <summary>
        /// 签名证书密码
        /// </summary>
        public const string SignCertPwd = "SignCertPwd";

        /// <summary>
        /// 验签证书目录
        /// </summary>
        public const string ValidateCertDir = "ValidateCertDir";

        /// <summary>
        /// 批量交易地址
        /// </summary>
        public const string BatTransUrl = "BatTransUrl";
    }
}
