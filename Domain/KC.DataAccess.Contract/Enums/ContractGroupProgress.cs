using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Enums.Contract
{
    public enum ContractGroupProgress
    {
        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        Success = 0,

        /// <summary>
        /// 推送合同到平台失败
        /// </summary>
        [Description("推送合同到平台失败")]
        UpdateToPlatFormFail = 1,

        /// <summary>
        /// 推送合同到签署公司失败
        /// </summary>
        [Description("推送合同到签署公司失败")]
        UpdateToOtherFail = 2,
 
        /// <summary>
        /// 删除合同到签署公司失败
        /// </summary>
        [Description("删除合同到签署公司失败")]
        DelToOtherFail = 3,
 
        /// <summary>
        /// 删除合同到发起人失败
        /// </summary>
        [Description("删除合同到发起人失败")]
        DelToCreateFail = 4,

        /// <summary>
        /// 签署合同更新业务数据异常
        /// </summary>
        [Description("签署合同更新业务数据异常")]
        UpdateToCallBackFail = 5,
        /// <summary>
        /// 下载合同
        /// </summary>
        [Description("下载合同")]
        DownContract = 6,
    }
}
