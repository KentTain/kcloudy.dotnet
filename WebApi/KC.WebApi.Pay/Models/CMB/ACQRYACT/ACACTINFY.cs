using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KC.WebApi.Pay.Models.CMB
{
    public class ACACTINFY : CMBBaseModel
    {
        /// <summary>
        /// 账户类型
        /// </summary>
        public string ACCFLG { get; set; }

        /// <summary>
        /// 实体账户
        /// </summary>
        public string ACTACC { get; set; }

        /// <summary>
        /// 余额
        /// </summary>
        public string ACTBAL { get; set; }

        /// <summary>
        /// 助记码
        /// </summary>
        public string ACTMNO { get; set; }

        /// <summary>
        /// 户名
        /// </summary>
        public string ACTNAM { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string ACTRMK { get; set; }

        /// <summary>
        /// 账户流水号
        /// </summary>
        public string ACTSEQ { get; set; }

        /// <summary>
        /// 附件标识 Y-有
        /// N-无
        /// </summary>
        public string ATHFLG { get; set; }

        /// <summary>
        /// 可用余额
        /// </summary>
        public string AVLBAL { get; set; }

        /// <summary>
        /// 账户种类
        /// </summary>
        public string BACKND { get; set; }

        /// <summary>
        /// 外部账号
        /// </summary>
        public string BACNBR { get; set; }

        /// <summary>
        /// 账户级别
        /// </summary>
        public string BACRNK { get; set; }

        /// <summary>
        /// 外部账号性质
        /// </summary>
        public string BACTYP { get; set; }

        /// <summary>
        /// 是否开通银企直连平台
        /// </summary>
        public string BCPFLG { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string BIGFLG { get; set; }

        public string BLGBNK { get; set; }

        public string BNKBMW { get; set; }

        public string BNKNBR { get; set; }

        public string BNKTYP { get; set; }

        public string BRDNBR { get; set; }

        public string BRNNBR { get; set; }

        public string CCYNBR { get; set; }

        public string CLTNAM { get; set; }

        public string CLTNBR { get; set; }

        public string CNLDAT { get; set; }

        public string CNLRES { get; set; }

        public string CNRCOD { get; set; }

        public string CTYCOD { get; set; }

        public string FATACT { get; set; }

        public string FIXFLG { get; set; }

        public string GRPBAL { get; set; }

        public string LUPDAT { get; set; }

        public string LUPTIM { get; set; }

        public string OPNBNK { get; set; }

        public string OPNCNL { get; set; }

        public string OPNDAT { get; set; }


        public string PACLMT { get; set; }

        public string PAYFLG { get; set; }

        public string PAYPRM { get; set; }

        public string PVCCOD { get; set; }

        public string RCBADR { get; set; }

        public string RCBSWF { get; set; }

        public string REVADR { get; set; }

        public string REVCNR { get; set; }

        public string RPTBAL { get; set; }

        public string SPARE1 { get; set; }

        public string STSCOD { get; set; }

        /// <summary>
        /// 备注1
        /// </summary>
        public string SPCRM1 { get; set; }

        public string SUMBAL { get; set; }

        public string VTLFLG { get; set; }
    }
}