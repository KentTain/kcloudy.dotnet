using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using KC.Service.DTO.Contract;
using KC.Enums.Contract;
using KC.Framework.Base;
using KC.Framework.Extension;
using KC.Framework.SecurityHelper;
using KC.Framework.Tenant;
using KC.Service.Contract;
using KC.Service.Contract.WebApiService.Business;
using KC.Storage.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using KC.Service.Contract.WebApiService;

namespace KC.Web.Contract.Controllers
{
    [Web.Extension.MenuFilter("合同管理", "合同管理", "/ContractManager/Index",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-file-text", AuthorityId = "B878EBB8-8CA2-4A0F-B88D-8ED33568DA99",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 3, IsExtPage = false, Level = 2)]
    public class ContractManagerController : ContractBaseController
    {
        protected ITenantSimpleApiService _tenantSimpleStore => ServiceProvider.GetService<ITenantSimpleApiService>();

        protected IContractService _contractService => ServiceProvider.GetService<IContractService>();
        protected IContractApiService _contractApiService => ServiceProvider.GetService<IContractApiService>();

        public ContractManagerController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<ContractManagerController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        [Web.Extension.PermissionFilter("合同管理", "合同管理", "/ContractManager/Index", "875540DC-9C97-4385-BF81-B9C6F8F1B91C",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsPage = true, ResultType = ResultType.ActionResult, AuthorityId = "B878EBB8-8CA2-4A0F-B88D-8ED33568DA99")]
        public ActionResult Index()
        {
            ViewBag.CurrentUserName = CurrentUserName;
            ViewBag.StatusList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnumWithAll<ContractStatus>();
            return View();
        }

        //[Web.Extension.PermissionFilter("合同管理", "加载合同列表", "/ContractManager/GetContractPageList", "8ABD436B-9141-4A59-9B27-1562D6627ACB",
        //    DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsPage = false, ResultType = ResultType.ContentResult, AuthorityId = "8ABD436B-9141-4A59-9B27-1562D6627ACB")]
        public async Task<JsonResult> GetContractPageList(int page, int rows, DateTime? startTime, DateTime? endTime, string key, ContractStatus? contractStatu, ContractType? contracttype, CustomerType? customerType)
        {
            var result = await _contractService.GetContractPageListAsync(page, rows, startTime, endTime, key, contractStatu, contracttype, customerType, CurrentUserId, CurrentUserTenantName, CurrentUserTenantDisplayName);
            return Json(result);
        }

        #region 新增编辑合同
        public PartialViewResult GetContractForm(Guid? id)
        {
            var model = new ContractGroupDTO();
            if(!id.HasValue)
            {
                model = _contractService.GetContractGroupById(id.Value);
                var blob = BlobUtil.GetBlobById(Tenant, CurrentUserId, model.BlobId);
                if (blob != null && blob.Data != null && blob.Data.Length > 0)
                {
                    model.ContractContent = Base64Provider.DecodeString(blob.FileName);
                }
                else
                {
                    model.ContractContent = "";
                }
            }

            ViewBag.ContractStatusList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<ContractStatus>();

            return PartialView("_contractForm", model);
        }

        [Web.Extension.PermissionFilter("合同管理", "保存合同", "/ContractManager/SaveContract", "61007910-90BF-418F-90EF-8F59E49107C7",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 4, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "61007910-90BF-418F-90EF-8F59E49107C7")]
        public JsonResult SaveContract(ContractGroupDTO model)
        {
            var msg = string.Empty;
            return GetServiceJsonResult(() =>
            {
                if (string.IsNullOrEmpty(model.ContractTitle))
                    throw new ArgumentNullException("ContractTitle", "合同标题不能为空");
                if (string.IsNullOrEmpty(model.ContractNo))
                    throw new ArgumentNullException("ContractNo", "合同编号不能为空");
                if (string.IsNullOrEmpty(model.BlobId))
                    throw new ArgumentNullException("ContractNo", "上传文件不能为空");

                var filename = model.UserName;
                model.UserId = TenantName;
                model.UserName = TenantDisplayName;
                //model.Type = ContractType.Lending;
                var editResult = _contractService.EditCurrencySignService(model, filename, CurrentUserId);
                if (!editResult.success)
                {
                    msg = editResult.message;
                    return false;
                }
                return true;
            });
        }

        [Web.Extension.PermissionFilter("合同管理", "获取单个合同", "/ContractManager/GetContract", "75AA0526-F232-4374-BA07-DB1B66077C14",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 5, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "75AA0526-F232-4374-BA07-DB1B66077C14")]
        public JsonResult GetContract(Guid id)
        {
            return GetServiceJsonResult(() =>
            {
                var data = _contractService.GetContractGroupById(id);
                if (data == null) { return null; }
                var blob = BlobUtil.GetBlobById(Tenant, CurrentUserId, data.BlobId);
                if (blob != null && blob.Data != null && blob.Data.Length > 0)
                {
                    data.ContractContent = Base64Provider.DecodeString(blob.FileName);
                }
                else
                {
                    data.ContractContent = "";
                }

                return data;
            });
        }

        public PartialViewResult Signing()
        {
            return PartialView("_signing");
        }

        /// <summary>
        /// 查找已认证的 企业用户
        /// </summary>
        public JsonResult GetUserList(string userName, Guid fromAppId, int page = 1, int rows = 10)
        {
            return GetServiceJsonResult(() =>
            {
                return _tenantSimpleStore.GetTenantUsersByOpenAppId(page, rows, fromAppId, userName, "");
            });
        }
        public JsonResult SearchEnterpriseEmployee(int page, int rows, string tName, string search)
        {
            if (string.IsNullOrEmpty(tName))
            {
                return Json(null);
            }

            var result = _contractApiService.SearchEnterpriseEmployee(page, rows, tName, search);
            if (result.success)
            {
                return Json(result.Result);
            }
            return Json(result);
        }
        #endregion

        #region 合同详情 (包括所有操作按钮)
        public PartialViewResult Details(Guid id, ContractOpt? opt, bool isPersonal = false)
        {
            var result = _contractService.GetContractGroupById(id);
            result.Opt = opt;
            if (isPersonal)
            {
                result.IsPersonal = isPersonal;
                result.CurrentUserId = CurrentUserId;
                result.CurrentTenantName = CurrentUserTenantName;
                result.CurrentUserDisplayName = CurrentUserDisplayName;
                result.CurrentUserPhone = CurrentUserPhone;
            }
            return PartialView("Details", result);
        }
        public PartialViewResult GetFDetails(string id, string opt = "")
        {
            ViewBag.opt = opt;
            var result = _contractService.GetContractGroupById(id);
            return PartialView("Details", result);
        }
        /// <summary>
        /// 对账单过来的
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PartialViewResult DetailsFromAS(string id)
        {
            var result = _contractService.GetContractGroupByRelationData(id);
            return PartialView("Details", result);
        }
        /// <summary>
        /// 合同详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult GetDetails(Guid id)
        {
            return GetServiceJsonResult(() =>
            {
                return _contractService.GetDetails(id);
            });
        }

        #endregion

        /// <summary>
        /// 作废合同
        /// </summary>
        /// <param name="id">签署用户Id</param>
        /// <param name="pdfX">作废章的X坐标</param>
        /// <param name="pdfY">作废章的Y坐标</param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public JsonResult AbandonedContract(Guid id, string remark = "", string pdfX = "", string pdfY = "")
        {
            return GetServiceJsonResult(() =>
            {
                return _contractService.AbandonedContract(CurrentUserName, CurrentUserDisplayName, id, remark, pdfX, pdfY, GlobalConfig.SSOWebDomain.Replace(TenantConstant.SubDomain, TenantName));
            });
        }

        /// <summary>
        /// 删除合同
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult RemoveCurrencySignService(Guid id)
        {
            return GetServiceJsonResult(() =>
            {
                return _contractService.RemoveCurrencySignService(id, CurrentUserName, CurrentUserDisplayName);
            });
        }

        #region 审核合同,退回合同

        /// <summary>
        /// 审核合同
        /// </summary>
        /// <returns></returns>
        public JsonResult ComfirmContract(Guid id, bool isComfirmFrist = false)
        {
            return GetServiceJsonResult(() =>
            {
                return _contractService.ComfirmContract(CurrentUserName, CurrentUserDisplayName, id, isComfirmFrist, GlobalConfig.PortalWebDomain);
            });
        }

        /// <summary>
        /// 退回
        /// </summary>
        /// <param name="id">合同Id</param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public JsonResult RetutrnContract(Guid id, string remark = "")
        {
            return GetServiceJsonResult(() =>
            {
                return _contractService.RetutrnContract(CurrentUserName, CurrentUserDisplayName, id, remark, GlobalConfig.PortalWebDomain.Replace(TenantConstant.SubDomain, TenantName), CurrentUserId);
            });
        }

        #endregion

        #region 盖在+验证+验证码

        /// <summary>
        /// 检验印章状态
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public JsonResult IsSeal(string token, bool? isPersonal)
        {
            var userid = "";
            if (isPersonal.HasValue && isPersonal.Value)
            {
                userid = CurrentUserId;
            }
            return GetServiceJsonResult(() =>
            {
                return _contractApiService.IsSeal(userid, TenantName);
            });
        }

        public JsonResult GetCode(bool? isPersonal)
        {
            return GetServiceJsonResult(() => 
            {
                return _contractApiService.GetCode(CurrentUserId, TenantName, isPersonal);
            });
        }


        public ActionResult GetCodeFormUrl(string phone, bool? isPersonal)
        {
            ViewBag.phone = phone;
            ViewBag.isPersonal = isPersonal;
            return PartialView("_signContractCodeFrom");
        }

        /// <summary>
        /// 盖章
        /// </summary>
        /// <param name="code"></param>
        /// <param name="id"></param>
        /// <param name="signType">签章类型:1单页签章,2多页签章，3骑缝章，4关键字签章</param>
        /// <param name="pdfX">pdf文件横坐标</param>
        /// <param name="pdfY">pdf文件纵坐标</param>
        /// <param name="posPage">第几页</param>
        /// <returns></returns>
        public JsonResult SignContract(string code, bool personal, Guid id, int pdfX = 0, int pdfY = 0, string posPage = "0", int signType = 1)
        {
            var url = GlobalConfig.PortalWebDomain.Replace(TenantConstant.SubDomain, TenantName);

            return GetServiceJsonResult(() =>
            {
                return _contractService.SignContractFun(CurrentUserId, CurrentUserName, CurrentUserDisplayName, personal, code, id, url, pdfX, pdfY, posPage, signType);
            });
        }

        #endregion

        #region 日志及相关操作
        /// <summary>
        /// 日志视图
        /// </summary>
        /// <returns></returns>
        public PartialViewResult CommonLog()
        {
            return PartialView();
        }

        public JsonResult LoadContractGroupLogs(Guid contractGroupId, int page = 1, int rows = 10)
        {
            var result = _contractService.LoadContractGroupLogs(contractGroupId, page, rows);
            return Json(result);
        }

        /// <summary>
        /// 同步数据
        /// </summary>
        /// <param name="contractGroupId">合同id</param>
        /// <param name="contractGroupProgress">进程错误步骤</param>
        /// <param name="processLogId">日志id</param>
        /// <returns></returns>
        public JsonResult SynchronousCurrencySignData(int contractGroupProgress, Guid contractGroupId, int processLogId)
        {
            var msg = "同步成功！";
            var result = GetServiceJsonResult(() =>
            {
                var contractGroup = _contractService.GetContractGroupById(contractGroupId);
                var resBool = true;
                if (processLogId == 0)//同步到平台失败
                {
                    if (_contractService.ReApplyPlatFormConfirmCurrencySign(CurrentUserName, CurrentUserDisplayName, contractGroup) <= 0)
                    {
                        msg = "操作失败,请重试！";
                        return false;
                    }
                }
                else
                {
                    if (contractGroupProgress == int.Parse(ContractGroupProgress.UpdateToOtherFail.ObjToDecimal().ToString()))
                    {
                        resBool = _contractService.ReApplyConfirmCurrencySign(contractGroup, CurrentUserId, CurrentUserName, processLogId);
                    }
                    else if (contractGroupProgress == int.Parse(ContractGroupProgress.DelToOtherFail.ObjToDecimal().ToString()))
                    {
                        resBool = _contractService.ReApplyRemoveCurrencySignService(contractGroup, CurrentUserId, CurrentUserName, processLogId);
                    }
                    else if (contractGroupProgress == int.Parse(ContractGroupProgress.DelToCreateFail.ObjToDecimal().ToString()))
                    {
                        resBool = _contractService.ReApplyRemoveCurrencySignService(contractGroup, CurrentUserId, CurrentUserName, processLogId, 1);
                    }
                    else if (contractGroupProgress == int.Parse(ContractGroupProgress.UpdateToCallBackFail.ObjToDecimal().ToString()))
                    {
                        var url = GlobalConfig.PortalWebDomain.Replace(TenantConstant.SubDomain, TenantName);
                        resBool = _contractService.HandleSignCallbackFail(contractGroupId, url);
                    }
                }
                if (!resBool)
                {
                    msg = "操作失败,请及时查看错误日志！";
                    return false;
                }
                return true;
            });
            //result.message = msg;
            return Json(result);
        }
        #endregion

    }
}
