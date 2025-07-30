using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Enums
{
    public enum ShowPage
    {
        /// <summary>
        /// 商城首页
        /// </summary>
        [Description("商城首页")]
        HomePage = 0,

        /// <summary>
        /// 产品首页
        /// </summary>
        [Description("产品首页")]
        OfferingHomePage = 1,

        /// <summary>
        /// 产品列表
        /// </summary>
        [Description("产品列表")]
        OfferingList = 2,

        /// <summary>
        /// 产品详细
        /// </summary>
        [Description("产品详细")]
        OfferingDetails = 3,

        /// <summary>
        /// 采购首页
        /// </summary>
        [Description("采购首页")]
        PurchaseHomePage = 4,

        /// <summary>
        /// 采购列表
        /// </summary>
        [Description("采购列表")]
        PurchaseList = 5,

        /// <summary>
        /// 采购详细
        /// </summary>
        [Description("采购详细")]
        PurchaseDetails = 6,

        /// <summary>
        /// 企业首页
        /// </summary>
        [Description("企业首页")]
        CompanyHomePage = 7,

        /// <summary>
        /// 企业列表
        /// </summary>
        [Description("企业列表")]
        CompanyList = 8
    }
}
