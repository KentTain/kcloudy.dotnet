using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using KC.Common;
using KC.Common.FileHelper;
using KC.Service.DTO.Contract;
using KC.Enums.Contract;
using KC.Framework.Base;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Service.Contract;
using KC.Service;
using KC.Service.Enums;
using KC.Storage.Util;
using KC.ThirdParty;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using KC.Web.Util;

namespace KC.Web.Contract.Controllers
{
    [Web.Extension.MenuFilter("合同管理", "合同模板管理", "/ContractTemplate/Index",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-file-text", AuthorityId = "D008E0EF-C39F-4387-B549-5DFDBD4EE471",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsExtPage = false, Level = 2)]
    public class ContractTemplateController : ContractBaseController
    {
        protected IContractService _templateService => ServiceProvider.GetService<IContractService>();

        public ContractTemplateController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<ContractTemplateController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        [Web.Extension.PermissionFilter("合同模板管理", "合同模板管理", "/ContractTemplate/Index", "875540DC-9C97-4385-BF81-B9C6F8F1B91C",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsPage = true, ResultType = ResultType.ActionResult, AuthorityId = "D008E0EF-C39F-4387-B549-5DFDBD4EE471")]
        public ActionResult Index()
        {
            ViewBag.CurrentUserName = CurrentUserName;
            ViewBag.StatusList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnumWithAll<ApprovalStatus>();
            return View();
        }

        //[Web.Extension.PermissionFilter("合同模板管理", "加载合同模板列表", "/ContractTemplate/LoadContractTemplate", "C795DEFD-FCB0-4D9A-A83F-ED22E1444785",
        //    DefaultRoleId = RoleConstants.AdminRoleId, Order = 4, IsPage = false, ResultType = ResultType.ContentResult, AuthorityId = "C795DEFD-FCB0-4D9A-A83F-ED22E1444785")]
        public JsonResult LoadContractTemplate()
        {
            var result = _templateService.LoadContractTemplates();
            return Json(result);
        }

        
        public PartialViewResult GetContractTemplateForm(int id)
        {
            var model = new ContractTemplateDTO();
            if(id == 0)
            {
                model = _templateService.GetContractTemplateById(id);
                ViewBag.ContractTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<ContractType>((int)model.Type);
            }
            else
            {
                ViewBag.ContractTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<ContractType>();
            }

            return PartialView("_contractTemplateForm", model);
        }

        [Web.Extension.PermissionFilter("合同模板管理", "保存合同模板", "/ContractTemplate/SaveContractTemplate", "B507C09C-BDF9-47E7-842B-A6FFC295B424",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 4, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "B507C09C-BDF9-47E7-842B-A6FFC295B424")]
        public JsonResult SaveContractTemplate(ContractTemplateValueDTO model)
        {
            return GetServiceJsonResult(() => {
                if (!(model.Day > 0 && model.BreakDay > 0))
                    throw new ArgumentNullException("Day", "该合同模板数据填写不完整，请先完善合同模板内容再生成合同");

                if (model.Id == 2 || model.Id == 3 || model.Id == 4 || model.Id == 5 || model.Id == 6)
                    if (!(model.AccountDayIn > 0))
                        throw new ArgumentNullException("Day", "该合同模板数据填写不完整，请先完善合同模板内容再生成合同");

                if (model.Id == 2 || model.Id == 4 || model.Id == 5 || model.Id == 6 || model.Id == 7)
                {
                    if (!(model.DeliveryDayIn > 0))
                        throw new ArgumentNullException("Day", "该合同模板数据填写不完整，请先完善合同模板内容再生成合同");
                    if (model.Id != 2)
                    {
                        if (!(model.DisputeType == "a" || model.DisputeType == "b"))
                            throw new ArgumentNullException("DisputeType", "解决争议方式只能选a或者b选择");
                        if (model.Id != 7 && string.IsNullOrEmpty(model.DeliveryMode))
                            throw new ArgumentNullException("DeliveryMode", "请完善交货方式");
                        if (!(model.HandleDay > 0 && model.ChangeContractDay > 0 && model.RectificDay > 0 && model.InvestiHandleDay > 0 && model.Rescheduling > 0))
                            throw new ArgumentNullException("Rescheduling", "该合同模板数据填写不完整，请先完善合同模板内容再生成销售合同");
                    }
                }

                if (model.Id == 7)
                    if (!(model.LeaseTerm > 0 && model.RentingDay > 0 && model.SecrecyPeriod > 0))
                        throw new ArgumentNullException("Rescheduling", "该合同模板数据填写不完整，请先完善合同模板内容再生成销售合同");

                if (string.IsNullOrEmpty(model.OpenBank) || string.IsNullOrEmpty(model.BankAccount) || string.IsNullOrEmpty(model.BankAccountName))
                    throw new ArgumentNullException("BankAccountName", "请完善开户行信息");

                return _templateService.SaveContractTemplate(model);
            });
        }


        [Web.Extension.PermissionFilter("合同模板管理", "删除合同模板", "/ContractTemplate/DeleteContractTemplate", "F31E418A-2C83-4D87-AC40-36F1DA1F2000",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 5, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "F31E418A-2C83-4D87-AC40-36F1DA1F2000")]
        public JsonResult DeleteContractTemplate(int sid)
        {
            return GetServiceJsonResult(() => _templateService.SoftRemoveContractTemplate(sid));
        }


        [Web.Extension.PermissionFilter("合同模板管理", "文件下载", "/ContractTemplate/DownloadContractTemplate", "FBDE66C9-9A80-4733-A536-A35B7F28C36B",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 8, IsPage = false, ResultType = ResultType.ContentResult, AuthorityId = "FBDE66C9-9A80-4733-A536-A35B7F28C36B")]
        public async Task<FileResult> DownloadContractTemplate(int id, ContractTemplateOrderDTO dto = null)
        {
            var result = await ViewEngineUtil.RenderToStringAsync(ServiceProvider, "Template/ContractTemplateView_"+ id, dto ?? new ContractTemplateOrderDTO());

            byte[] pdfFile = PdfHelper.ConvertHtmlTextToPdf(result);
            return File(pdfFile, "application/pdf", HttpUtility.UrlEncode("Demo.pdf", Encoding.UTF8));
        }

        [HttpPost]
        public async Task<JsonResult> OrderUpPDFToContract(int id, string orderInfoId, string contractNo)
        {
            #region 判断条件
            if (string.IsNullOrEmpty(contractNo))
                return ThrowErrorJsonMessage(false, "请输入合同编号");

            //检查模板是否补充完整
            var tempModel = await _templateService.GetMyContractTemplateById(id);
            if (tempModel == null)
            {
                return ThrowErrorJsonMessage(false, "销售合同模板数据有误");
            }
            if (!(tempModel.Day > 0 && tempModel.BreakDay > 0))
            {
                return ThrowErrorJsonMessage(false, "该合同模板数据填写不完整，请先完善合同模板内容再生成销售合同");
            }
            if (tempModel.Id == 2 || tempModel.Id == 3 || tempModel.Id == 4 || tempModel.Id == 5 || tempModel.Id == 6)
            {
                if (!(tempModel.AccountDayIn > 0))
                {
                    return ThrowErrorJsonMessage(false, "该合同模板数据填写不完整，请先完善合同模板内容再生成销售合同");
                }
            }
            if (tempModel.Id == 2 || tempModel.Id == 4 || tempModel.Id == 5 || tempModel.Id == 6 || tempModel.Id == 7)
            {
                if (!(tempModel.DeliveryDayIn > 0))
                {
                    return ThrowErrorJsonMessage(false, "该合同模板数据填写不完整，请先完善合同模板内容再生成销售合同");
                }
                if (tempModel.Id != 2)
                {
                    if (!(tempModel.DisputeType == "a" || tempModel.DisputeType == "b"))
                    {
                        return ThrowErrorJsonMessage(false, "解决争议方式只能选a或者b选择");
                    }
                    if (tempModel.Id != 7 && string.IsNullOrEmpty(tempModel.DeliveryMode))
                    {
                        return ThrowErrorJsonMessage(false, "请完善交货方式");
                    }
                    if (!(tempModel.HandleDay > 0 && tempModel.ChangeContractDay > 0 && tempModel.RectificDay > 0 && tempModel.InvestiHandleDay > 0 && tempModel.Rescheduling > 0))
                    {
                        return ThrowErrorJsonMessage(false, "该合同模板数据填写不完整，请先完善合同模板内容再生成销售合同");
                    }
                }
            }
            if (tempModel.Id == 7)
            {
                if (!(tempModel.LeaseTerm > 0 && tempModel.RentingDay > 0 && tempModel.SecrecyPeriod > 0))
                {
                    return ThrowErrorJsonMessage(false, "该合同模板数据填写不完整，请先完善合同模板内容再生成销售合同");
                }
            }
            if (string.IsNullOrEmpty(tempModel.OpenBank) || string.IsNullOrEmpty(tempModel.BankAccount) || string.IsNullOrEmpty(tempModel.BankAccountName))
            {
                return ThrowErrorJsonMessage(false, "请完善开户行信息");
            }
            #endregion

            #region 创建pdf
            var blobId = Guid.NewGuid().ToString();
            var title = "交易合同" + orderInfoId;
            WebClient wc = new WebClient();
            wc.Headers.Add("Accept-Language", "zh-cn");
            wc.Encoding = System.Text.Encoding.UTF8;
            string htmlText = wc.DownloadString(GlobalConfig.EconWebDomain.Replace(TenantConstant.SubDomain, TenantName) + "Template/ContractTemplateView" + "?id=" + id + "&orderInfoId=" + orderInfoId + "&contractNo=" + contractNo);
            byte[] pdfFile = PdfHelper.ConvertHtmlTextToPdf(htmlText);

            var blobInfo = new BlobInfo(blobId, FileType.PDF.ToString(), DocFormat.Pdf.ToString(), title, "pdf", pdfFile);
            if (!BlobUtil.SaveBlob(Tenant, TenantName, blobInfo))
            {
                return ThrowErrorJsonMessage(false, "合同文件保存失败");
            }
            #endregion
            return GetServiceJsonResult(() => _templateService.OrderUpPDFToContract(orderInfoId, CurrentUserId, CurrentUserName, title, blobId, contractNo));
        }

        [AllowAnonymous]
        public ActionResult AddValueServiceContract(string contractNo, decimal allMoney)
        {
            var key = Tenant.TenantName + "SystemAppreciationDTO";
            var model = CacheUtil.GetCache<string>(key);
            if (string.IsNullOrWhiteSpace(model))
            {
                return View();
            }

            CacheUtil.RemoveCache(key);
            ViewBag.ContractNo = contractNo;
            // 甲方法人信息
            //var authInfo = AccountService.GetEnterpriseIdentityAuthenticationByMemberId(TenantName);
            //if (authInfo != null && authInfo.AuthInfo != null)
            //{
            //    ViewBag.CompanyName = authInfo.AuthInfo.CompanyName;
            //    ViewBag.LegalPerson = authInfo.AuthInfo.LegalPerson;
            //    ViewBag.UnifiedSocialCreditCode = authInfo.AuthInfo.UnifiedSocialCreditCode;
            //    ViewBag.CompanyAddress = authInfo.AuthInfo.CompanyAddress;
            //}
            ViewBag.AllMoney = allMoney;
            ViewBag.AllMoneyStr = MoneyToUpper(allMoney.ToString());
            ViewBag.SystemAppreciationDTO = SerializeHelper.FromJson<List<SystemAppreciationDTO>>(model);
            return View();
        }

        [HttpPost]
        public JsonResult MakeAddValueServiceContract(List<SystemAppreciationDTO> model)
        {
            //包含定制的项目
            if (model.Any(m => m.Id >= 13))
            {
                //发送短信
                string smsMessage = string.Format("{0}向贵司提交了定制化增值服务申请，服务内容为：云服务定制，请联系客户进行跟进。", TenantDisplayName);
                //_templateService.SendAddServiceMessage(smsMessage);
            }

            //存在自选的项目去生成合同
            if (model.Any(m => m.Id < 13))
            {
                #region 创建pdf
                var contractNo = string.Empty;//OtherUtilHelper.GetSerialNumber(SequenceNumberConstants.TYHT);
                var blobId = Guid.NewGuid().ToString();
                var title = "云服务协议";

                decimal allMoney = 0;
                if (model != null && model.Any())
                {
                    allMoney = model.Sum(m => m.Amount);
                }
                else
                {
                    return ThrowErrorJsonMessage(false, "请选择增值服务");
                }
                var key = Tenant.TenantName + "SystemAppreciationDTO";
                CacheUtil.SetCache(key, SerializeHelper.ToJson(model));

                WebClient wc = new WebClient();
                wc.Headers.Add("Accept-Language", "zh-cn");
                wc.Encoding = System.Text.Encoding.UTF8;
                var result = wc.DownloadString(GlobalConfig.EconWebDomain.Replace(TenantConstant.SubDomain, TenantName) + "Template/AddValueServiceContract?contractNo=" + contractNo + "&allMoney=" + allMoney);
                if (string.IsNullOrEmpty(result))
                {
                    return ThrowErrorJsonMessage(false, "系统异常");
                }

                byte[] pdfFile = PdfHelper.ConvertHtmlTextToPdf(result);
                var blobInfo = new BlobInfo(blobId, FileType.PDF.ToString(), DocFormat.Pdf.ToString(), title, "pdf", pdfFile);
                if (!BlobUtil.SaveBlob(Tenant, TenantName, blobInfo))
                {
                    return ThrowErrorJsonMessage(false, "合同文件保存失败");
                }
                #endregion

                return GetServiceJsonResult(() => _templateService.MakeAddValueServiceContract(CurrentUserId, CurrentUserName, title, blobId, contractNo, allMoney));
            }
            return GetServiceJsonResult(() =>
            {
                return true;
            });
        }

        #region 金额转换成中文大写金额
        /// <summary>
        /// 金额转换成中文大写金额
        /// </summary>
        /// <param name="LowerMoney">eg:10.74</param>
        /// <returns></returns>
        public static string MoneyToUpper(string LowerMoney)
        {
            string functionReturnValue = null;
            bool IsNegative = false; // 是否是负数
            if (LowerMoney.Trim().Substring(0, 1) == "-")
            {
                // 是负数则先转为正数
                LowerMoney = LowerMoney.Trim().Remove(0, 1);
                IsNegative = true;
            }
            string strLower = null;
            string strUpart = null;
            string strUpper = null;
            int iTemp = 0;
            // 保留两位小数 123.489→123.49　　123.4→123.4
            LowerMoney = Math.Round(double.Parse(LowerMoney), 2).ToString();
            if (LowerMoney.IndexOf(".") > 0)
            {
                if (LowerMoney.IndexOf(".") == LowerMoney.Length - 2)
                {
                    LowerMoney = LowerMoney + "0";
                }
            }
            else
            {
                LowerMoney = LowerMoney + ".00";
            }
            strLower = LowerMoney;
            iTemp = 1;
            strUpper = "";
            while (iTemp <= strLower.Length)
            {
                switch (strLower.Substring(strLower.Length - iTemp, 1))
                {
                    case ".":
                        strUpart = "元";
                        break;
                    case "0":
                        strUpart = "零";
                        break;
                    case "1":
                        strUpart = "壹";
                        break;
                    case "2":
                        strUpart = "贰";
                        break;
                    case "3":
                        strUpart = "叁";
                        break;
                    case "4":
                        strUpart = "肆";
                        break;
                    case "5":
                        strUpart = "伍";
                        break;
                    case "6":
                        strUpart = "陆";
                        break;
                    case "7":
                        strUpart = "柒";
                        break;
                    case "8":
                        strUpart = "捌";
                        break;
                    case "9":
                        strUpart = "玖";
                        break;
                }

                switch (iTemp)
                {
                    case 1:
                        strUpart = strUpart + "分";
                        break;
                    case 2:
                        strUpart = strUpart + "角";
                        break;
                    case 3:
                        strUpart = strUpart + "";
                        break;
                    case 4:
                        strUpart = strUpart + "";
                        break;
                    case 5:
                        strUpart = strUpart + "拾";
                        break;
                    case 6:
                        strUpart = strUpart + "佰";
                        break;
                    case 7:
                        strUpart = strUpart + "仟";
                        break;
                    case 8:
                        strUpart = strUpart + "万";
                        break;
                    case 9:
                        strUpart = strUpart + "拾";
                        break;
                    case 10:
                        strUpart = strUpart + "佰";
                        break;
                    case 11:
                        strUpart = strUpart + "仟";
                        break;
                    case 12:
                        strUpart = strUpart + "亿";
                        break;
                    case 13:
                        strUpart = strUpart + "拾";
                        break;
                    case 14:
                        strUpart = strUpart + "佰";
                        break;
                    case 15:
                        strUpart = strUpart + "仟";
                        break;
                    case 16:
                        strUpart = strUpart + "万";
                        break;
                    default:
                        strUpart = strUpart + "";
                        break;
                }

                strUpper = strUpart + strUpper;
                iTemp = iTemp + 1;
            }

            strUpper = strUpper.Replace("零拾", "零");
            strUpper = strUpper.Replace("零佰", "零");
            strUpper = strUpper.Replace("零仟", "零");
            strUpper = strUpper.Replace("零零零", "零");
            strUpper = strUpper.Replace("零零", "零");
            strUpper = strUpper.Replace("零角零分", "整");
            strUpper = strUpper.Replace("零分", "整");
            strUpper = strUpper.Replace("零角", "零");
            strUpper = strUpper.Replace("零亿零万零元", "亿元");
            strUpper = strUpper.Replace("亿零万零元", "亿元");
            strUpper = strUpper.Replace("零亿零万", "亿");
            strUpper = strUpper.Replace("零万零元", "万元");
            strUpper = strUpper.Replace("零亿", "亿");
            strUpper = strUpper.Replace("零万", "万");
            strUpper = strUpper.Replace("零元", "元");
            strUpper = strUpper.Replace("零零", "零");

            // 对壹元以下的金额的处理
            if (strUpper.Substring(0, 1) == "元")
            {
                strUpper = strUpper.Substring(1, strUpper.Length - 1);
            }
            if (strUpper.Substring(0, 1) == "零")
            {
                strUpper = strUpper.Substring(1, strUpper.Length - 1);
            }
            if (strUpper.Substring(0, 1) == "角")
            {
                strUpper = strUpper.Substring(1, strUpper.Length - 1);
            }
            if (strUpper.Substring(0, 1) == "分")
            {
                strUpper = strUpper.Substring(1, strUpper.Length - 1);
            }
            if (strUpper.Substring(0, 1) == "整")
            {
                strUpper = "零元整";
            }
            functionReturnValue = strUpper;

            if (IsNegative == true)
            {
                return "负" + functionReturnValue;
            }
            else
            {
                return functionReturnValue;
            }
        }
        #endregion
    }
}
