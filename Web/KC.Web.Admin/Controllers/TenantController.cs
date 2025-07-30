using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KC.Enums.App;
using KC.Framework.Base;
using KC.Framework.Exceptions;
using KC.Framework.Extension;
using KC.Framework.SecurityHelper;
using KC.Framework.Tenant;
using KC.Model.Component.Queue;
using KC.Service.Admin;
using KC.Service.DTO.Admin;
using KC.Service.Component;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace KC.Web.Admin.Controllers
{
    public class TenantController : AdminBaseController
    {
        protected IDatabasePoolService DatabasePoolService => ServiceProvider.GetService<IDatabasePoolService>();
        protected IStoragePoolService StoragePoolService => ServiceProvider.GetService<IStoragePoolService>();
        protected IQueuePoolService QueuePoolService => ServiceProvider.GetService<IQueuePoolService>();
        protected INoSqlPoolService NoSqlPoolService => ServiceProvider.GetService<INoSqlPoolService>();
        protected IVodPoolService VodPoolService => ServiceProvider.GetService<IVodPoolService>();
        protected ICodePoolService CodePoolService => ServiceProvider.GetService<ICodePoolService>();
        protected IServiceBusPoolService ServiceBusPoolService => ServiceProvider.GetService<IServiceBusPoolService>();
        public TenantController(
            IServiceProvider serviceProvider,
            ILogger<TenantController> logger)
            : base(serviceProvider, logger)
        {
        }

        #region 租户列表
        [Web.Extension.MenuFilter("租户管理", "租户列表", "/Tenant/Index",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-users", AuthorityId = "986F9904-FC51-4AF4-A369-7E1D4855D61A",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("租户列表", "租户列表", "/Tenant/Index", "986F9904-FC51-4AF4-A369-7E1D4855D61A",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsPage = true, ResultType = ResultType.ActionResult, AuthorityId = "986F9904-FC51-4AF4-A369-7E1D4855D61A")]
        public IActionResult Index()
        {
            ViewBag.CloudTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnumWithAll<CloudType>();
            ViewBag.TenantVersionList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnumWithAll<Framework.Tenant.TenantVersion>();
            return View();
        }


        //[Web.Extension.PermissionFilter("租户列表", "加载租户列表数据", "/Tenant/LoadTenantUserList", "0BA88B88-048A-40AB-BA02-5648A9754DCF",
        //    DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "0BA88B88-048A-40AB-BA02-5648A9754DCF")]
        public IActionResult LoadTenantUserList(int page, int rows, int? cloudType, int? version, string searchKey = "", string searchValue = "")
        {
            string server = string.Empty;
            string userCode = string.Empty;
            string userName = string.Empty;
            string contact = string.Empty;
            if (!string.IsNullOrWhiteSpace(searchKey)
                && !string.IsNullOrWhiteSpace(searchValue))
            {
                switch (searchKey.ToLower())
                {
                    case "server":
                        server = searchValue.Trim();
                        break;
                    case "tenantcode":
                        userCode = searchValue.Trim();
                        break;
                    case "tenantname":
                        userName = searchValue.Trim();
                        break;
                    case "contactname":
                        contact = searchValue.Trim();
                        break;
                }
            }

            var result = TenantService.FindTenantUsersByFilter(page, rows, cloudType, version, userCode, contact, userName);

            return Json(result);
        }

        public PartialViewResult GetTenantUserForm(int id = 0)
        {
            ViewBag.StorageList = GetStorageList();
            ViewBag.DatabaseList = GetDatabaseList();
            ViewBag.QueuePoolList = GetQueuePoolList();
            ViewBag.NoSqlPoolList = GetNoSqlPoolList();
            ViewBag.VodPoolList = GetVodPoolList();
            ViewBag.CodePoolList = GetCodePoolList();
            
            var model = new TenantUserDTO() { IsEditMode = false };
            if (id != 0)
            {
                model = TenantService.GetTenantUserById(id);
                model.IsEditMode = true;
                ViewBag.CloudTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<CloudType>((int)model.CloudType);
                ViewBag.VersionList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<TenantVersion>((int)model.Version);
                ViewBag.CreditLimitTallyList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<TenantType>((int)model.TenantType);
            }
            else
            {
                ViewBag.CloudTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnumWithAll(new List<CloudType>());
                ViewBag.VersionList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnumWithAll(new List<TenantVersion>());
                ViewBag.CreditLimitTallyList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnumWithAll(new List<TenantType>());
            }

            return PartialView("_tenantUserForm", model);
        }

        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public IActionResult ExistTenantName(string tenantName, string orginalName, bool isEditMode)
        {
            if (isEditMode && tenantName.Equals(orginalName, StringComparison.OrdinalIgnoreCase))
            {
                return Json(true);
            }

            var result = TenantService.ExistTenantName(tenantName);
            return Json(!result);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Web.Extension.PermissionFilter("租户列表", "保存租户信息", "/Tenant/SaveTenantUserForm", "BDA3F21F-16D9-4DC6-99F8-0F7A57DC4980",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "BDA3F21F-16D9-4DC6-99F8-0F7A57DC4980")]
        public JsonResult SaveTenantUserForm(TenantUserDTO model)
        {
            return GetServiceJsonResult(() =>
            {
                if (!model.IsEditMode)
                {
                    model.CreatedBy = CurrentUserId;
                    model.CreatedName = CurrentUserDisplayName;
                    model.CreatedDate = DateTime.UtcNow;
                }
                model.ModifiedBy = CurrentUserId;
                model.ModifiedName = CurrentUserDisplayName;
                model.ModifiedDate = DateTime.UtcNow;

                var isSuccess = TenantService.SaveTenantUser(model);
                return isSuccess;
            });
        }
        [Web.Extension.PermissionFilter("租户列表", "删除租户列表数据", "/Tenant/RemoveTenantUser", "D3D82D15-B144-4A28-8DE6-C06CFA870ED0",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 3, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "D3D82D15-B144-4A28-8DE6-C06CFA870ED0")]
        public JsonResult RemoveTenantUser(List<int> ids)
        {
            return GetServiceJsonResult(() =>
            {
                return TenantService.SoftRemoveTenantUser(ids);
            });
        }

        public JsonResult RemoveTenantUserByName(List<string> ids)
        {
            return GetServiceJsonResult(() =>
            {
                return TenantService.SoftRemoveTenantUserByNames(ids);
            });
        }
        [Web.Extension.PermissionFilter("租户列表", "添加租户应用", "/Tenant/SavaTenantUserApplications", "57165E12-3DE5-4B88-B16A-ED5392E4AA57",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 4, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "57165E12-3DE5-4B88-B16A-ED5392E4AA57")]
        public JsonResult SavaTenantUserApplications(int tenantId, List<string> appplicationIds)
        {
            if (null == appplicationIds || !appplicationIds.Any())
                return base.ThrowErrorJsonMessage(false, "应用Id为空。");

            return GetServiceJsonResult(() =>
            {
                var appGuids = appplicationIds.ConvertAll(m => new Guid(m));
                var existsAppIds = TenantService.FindTenantApplicationIdsByTenantId(tenantId);
                var openAppIds = appGuids.Except(existsAppIds).ToList();

                return TenantService.AddTenantUserApplications(tenantId, openAppIds);
            });
        }

        #region 开通应用

        public JsonResult LoadTenantAppList(int tenantId)
        {
            var result = TenantService.FindTenantApplicationsByTenantId(tenantId);
            return Json(result);
        }

        public JsonResult GetTenantAppStatus(int tenantId, Guid appId)
        {
            var result = TenantService.GetTenantApplicationByFilter(tenantId, appId);
            return Json(result);
        }
        [Web.Extension.PermissionFilter("租户列表", "开通租户应用", "/Tenant/SavaTenantUserApplications", "381C3080-BD0B-47C7-B3AE-CADD6B17A5BE",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 5, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "381C3080-BD0B-47C7-B3AE-CADD6B17A5BE")]
        public JsonResult OpenApplicationService(int tenantId, string appId)
        {
            var t = new Thread(GeneratDataBase);
            t.Start(new ThreadParams() { TenantId = tenantId, AppId = appId, TenantService = TenantService });

            return base.ThrowErrorJsonMessage(true, "保存数据成功");
        }

        private void GeneratDataBase(object threadParams)
        {
            try
            {
                var tenantParams = threadParams as ThreadParams;
                if (tenantParams != null)
                {
                    tenantParams.TenantService.GeneratDataBaseAndOpenWebServer(tenantParams.TenantId, tenantParams.AppId);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
            }
        }

        #endregion

        #region 开通服务
        [Web.Extension.PermissionFilter("租户列表", "开通短信服务", "/Tenant/OpenSmsService", "B0F53C31-8974-48EF-B161-33B6BC01D7A9",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 6, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "B0F53C31-8974-48EF-B161-33B6BC01D7A9")]
        public JsonResult OpenSmsService(int tenantId, string tenantName)
        {
            return base.GetServiceJsonResult<bool>(() =>
            {
                var messge = TenantService.OpenEmailService(tenantId, tenantName);
                if (!string.IsNullOrWhiteSpace(messge))
                    throw new ComponentException(messge);
                return true;
            });
        }
        [Web.Extension.PermissionFilter("租户列表", "开通邮件服务", "/Tenant/OpenEmailService", "5CC95124-4311-49D3-A286-D82D401E8474",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 7, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "5CC95124-4311-49D3-A286-D82D401E8474")]
        public JsonResult OpenEmailService(int tenantId, string tenantName)
        {
            return base.GetServiceJsonResult<bool>(() =>
            {
                var messge = TenantService.OpenSmsService(tenantId, tenantName);
                if (!string.IsNullOrWhiteSpace(messge))
                    throw new ComponentException(messge);
                return true;
            });
        }
        [Web.Extension.PermissionFilter("租户列表", "开通呼叫中心服务", "/Tenant/OpenCallCenterService", "CAD430DF-8C50-4222-8174-758C2141121E",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 8, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "CAD430DF-8C50-4222-8174-758C2141121E")]
        public JsonResult OpenCallCenterService(int tenantId, string tenantName)
        {
            return base.GetServiceJsonResult<bool>(() =>
            {
                var messge = TenantService.OpenCallCenterService(tenantId, tenantName);
                if (!string.IsNullOrWhiteSpace(messge))
                    throw new ComponentException(messge);
                return true;
            });
        }

        #endregion

        #region 数据库操作
        [Web.Extension.PermissionFilter("租户列表", "更新租户数据库", "/Tenant/UpgradeTenantDatabase", "EB81D087-51C9-41C3-AE87-1B4ED196FC75",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 9, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "EB81D087-51C9-41C3-AE87-1B4ED196FC75")]
        public JsonResult UpgradeTenantDatabase(int tenantId, int id = 0)
        {
            return GetServiceJsonResult(() =>
            {
                var message = TenantService.UpgradeTenantDatabase(tenantId);
                if (!string.IsNullOrEmpty(message))
                    throw new ArgumentException(message);

                return id != 0
                    ? TenantService.UpdateTenantErrorLogStatus(id)
                    : string.IsNullOrEmpty(message);
            });
        }

        [Web.Extension.PermissionFilter("租户列表", "回滚所有租户数据库", "/Tenant/RollBackAllTenantDatabase", "79C48640-AA83-4D35-AADD-FD284619E355",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 10, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "79C48640-AA83-4D35-AADD-FD284619E355")]
        public JsonResult RollBackAllTenantDatabase()
        {
            var errors1 = TenantService.RollBackAllTenantDatabase();
            if (errors1.Any())
            {
                string message = "开通Web服务器出错，错误消息：" + string.Join(". " + Environment.NewLine, errors1);
                Logger.LogError(message);
                //EmailUtil.SendAdministratorMail("开通Web服务器出错", message);
            }

            return ThrowErrorJsonMessage(!errors1.Any(),
                errors1.Any()
                    ? "开通Web服务器出错，错误消息：" + string.Join(". " + Environment.NewLine, errors1)
                    : string.Empty);
        }

        [Web.Extension.PermissionFilter("租户列表", "回滚单个租户数据库", "/Tenant/RollBackDatabaseService", "B9A63EA0-BEEF-4437-B2D9-BEC51896C4AF",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 11, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "B9A63EA0-BEEF-4437-B2D9-BEC51896C4AF")]
        public JsonResult RollBackDatabaseService(int tenantId, string appId)
        {
            return base.GetServiceJsonResult<bool>(() =>
            {
                var messge = TenantService.RollBackTenantDataBase(tenantId, appId);
                if (!string.IsNullOrWhiteSpace(messge))
                    throw new ComponentException(messge);
                return true;
            });
        }
        #endregion

        #region 发送开通邮件及短信

        /// <summary>
        /// 发送开通服务短信
        /// </summary>
        /// <param name="tenantNames"></param>
        /// <returns></returns>
        [Web.Extension.PermissionFilter("租户列表", "发送开通短信", "/Tenant/SendOpenSms", "6FCA4967-CE65-4B54-994A-D90F7C25C633",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 12, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "6FCA4967-CE65-4B54-994A-D90F7C25C633")]
        public JsonResult SendOpenSms(List<string> tenantNames)
        {
            return GetServiceJsonResult(() =>
            {
                var data = TenantService.FindTenantsByNames(tenantNames);
                StringBuilder messages = new StringBuilder();

                foreach (var tenant in data)
                {
                    long phone;
                    var smsContent = string.Format("【财富共赢】尊敬的{0}：您的产融协作服务已开通，详情已发至邮箱：{1}。", tenant.TenantDisplayName,
                        tenant.ContactEmail);
                    if (long.TryParse(tenant.ContactPhone, out phone))
                    {
                        var smsInfo = new SmsInfo
                        {
                            Tenant = tenant.TenantDisplayName,
                            Type = SmsType.Marketing,
                            Phone = new List<long> { phone },
                            SmsContent = smsContent
                        };
                        var isSucceed = new StorageQueueService().InsertSmsQueue(smsInfo);
                        if (!isSucceed)
                        {
                            string errorMessage = tenant.TenantDisplayName + "：插入短信队列失败。";
                            TenantService.AddOpenAppErrorLog(tenant.TenantId, tenant.TenantDisplayName,
                                OpenServerType.SendOpenSms, errorMessage);
                            messages.AppendLine(errorMessage);
                        }
                        else
                        {
                            Logger.LogInformation(string.Format("开通短信已成功发送给租户【{0}】，短信内容：{1}", tenant.TenantDisplayName,
                                smsInfo.SmsContent));
                        }
                    }
                }
                var message = messages.ToString();
                if (!string.IsNullOrEmpty(message))
                    throw new ComponentException(message);

                return string.IsNullOrEmpty(message);
            });
        }
        [Web.Extension.PermissionFilter("租户列表", "发送开通邮件", "/Tenant/SendOpenEmail", "8436972E-B2B6-45F3-A4CA-29251B46201C",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 13, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "8436972E-B2B6-45F3-A4CA-29251B46201C")]
        public JsonResult SendOpenEmail(string tenantName)
        {
            return GetServiceJsonResult(() =>
            {
                var data = TenantService.GetTenantUserByName(tenantName);

                return TenantService.SendTenantAppOpenEmail(data);
            });
        }

        #endregion

        #region 测试连通性
        //[Web.Extension.PermissionFilter("租户列表", "测试数据库连接", "/Tenant/TestTenantDBConnection", "2AFAB010-25DD-4D93-A1C9-E3AD998864D6",
        //    DefaultRoleId = RoleConstants.AdminRoleId, Order = 15, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "2AFAB010-25DD-4D93-A1C9-E3AD998864D6")]
        public JsonResult TestTenantDBConnection(int tenantId)
        {
            return GetServiceJsonResult(() =>
            {
                var tenant = TenantService.GetTenantUserById(tenantId);
                var model = new DatabasePoolDTO()
                {
                    Server = tenant.Server,
                    Database = tenant.Database,
                    UserName = tenant.TenantName,
                    UserPasswordHash = tenant.DatabasePasswordHash,
                };

                return DatabasePoolService.TestDatabaseConnection(model, tenant.PrivateEncryptKey);
            });
        }
        //[Web.Extension.PermissionFilter("租户列表", "测试存储连接", "/Tenant/TestTenantStorageConnection", "CA7B9C5A-BBF6-4166-9A39-7361D4BF480D",
        //    DefaultRoleId = RoleConstants.AdminRoleId, Order = 16, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "CA7B9C5A-BBF6-4166-9A39-7361D4BF480D")]
        public JsonResult TestTenantStorageConnection(int tenantId)
        {
            return GetServiceJsonResult(() =>
            {
                var tenant = TenantService.GetTenantUserById(tenantId);
                var model = new StoragePoolDTO()
                {
                    CloudType = tenant.CloudType,
                    Endpoint = tenant.StorageEndpoint,
                    AccessName = tenant.StorageAccessName,
                    AccessKeyPasswordHash = tenant.StorageAccessKeyPasswordHash,
                };
                var rootPath = AppDomain.CurrentDomain.BaseDirectory;
                var filePath = rootPath + @"Content\images\404.png";
                return StoragePoolService.TestStorageConnection(model, filePath, tenant.PrivateEncryptKey);
            });
        }

        #endregion

        private IEnumerable<SelectListItem> GetStorageList()
        {
            var storageList = StoragePoolService.FindAllStoragePools();

            return
                storageList.Select(
                    item => new SelectListItem { Text = item.AccessName, Value = item.StoragePoolId.ToString() });
        }
        private IEnumerable<SelectListItem> GetDatabaseList()
        {
            var databaseList = DatabasePoolService.FindAllDatabasePools();

            return
                databaseList.Select(
                    item =>
                        new SelectListItem
                        {
                            Text = string.Format("服务器（{0}）下的数据库：{1}", item.Server, item.Database),
                            Value = item.DatabasePoolId.ToString()
                        });
        }
        private IEnumerable<SelectListItem> GetQueuePoolList()
        {
            var queueList = QueuePoolService.FindAllQueuePools();

            return
                queueList.Select(
                    item => new SelectListItem { Text = item.AccessName, Value = item.QueuePoolId.ToString() });
        }
        private IEnumerable<SelectListItem> GetNoSqlPoolList()
        {
            var queueList = NoSqlPoolService.FindAllNoSqlPools();

            return
                queueList.Select(
                    item => new SelectListItem { Text = item.AccessName, Value = item.NoSqlPoolId.ToString() });
        }
        private IEnumerable<SelectListItem> GetVodPoolList()
        {
            var queueList = VodPoolService.FindAllVodPools();

            return
                queueList.Select(
                    item => new SelectListItem { Text = item.AccessName, Value = item.VodPoolId.ToString() });
        }
        private IEnumerable<SelectListItem> GetCodePoolList()
        {
            var queueList = CodePoolService.FindAllCodeRepositoryPools();

            return
                queueList.Select(
                    item => new SelectListItem { Text = item.AccessName, Value = item.CodePoolId.ToString() });
        }
        #endregion

        #region 租户详情

        [Web.Extension.MenuFilter("租户管理", "租户详情", "/Tenant/TenantDetail",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-database", AuthorityId = "C531F825-9B0F-4ECC-A3C9-5C2EE006E267",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsExtPage = true, Level = 3)]
        [Web.Extension.PermissionFilter("租户列表", "租户详情", "/Tenant/TenantDetail", "C531F825-9B0F-4ECC-A3C9-5C2EE006E267",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 3, IsPage = true, ResultType = ResultType.ActionResult, AuthorityId = "C531F825-9B0F-4ECC-A3C9-5C2EE006E267")]
        public IActionResult TenantDetail(int id)
        {
            var model = TenantService.GetTenantUserById(id);
            var type = model.TypeString;
            var version = model.VersionString;
            ViewBag.SSOUrl = GlobalConfig.SSOWebDomain.Replace(TenantConstant.SubDomain, model.TenantName).ToLower();
            ViewBag.DatabasePassword = EncryptPasswordUtil.DecryptPassword(model.DatabasePasswordHash,
                !string.IsNullOrEmpty(model.PrivateEncryptKey) ? model.PrivateEncryptKey : null);
            ViewBag.StorageDeString = EncryptPasswordUtil.DecryptPassword(model.StorageAccessKeyPasswordHash,
                !string.IsNullOrEmpty(model.PrivateEncryptKey) ? model.PrivateEncryptKey : null);
            ViewBag.VodDeString = EncryptPasswordUtil.DecryptPassword(model.VodAccessKeyPasswordHash,
                !string.IsNullOrEmpty(model.PrivateEncryptKey) ? model.PrivateEncryptKey : null);
            ViewBag.CodeDeString = EncryptPasswordUtil.DecryptPassword(model.CodeAccessKeyPasswordHash,
                !string.IsNullOrEmpty(model.PrivateEncryptKey) ? model.PrivateEncryptKey : null);
            ViewBag.AdminAccount = "admin@cfwin.com";
            InitTenantApplicationHost(ref model);
            return View(model);
        }

        public IActionResult ModifyPhoneAndEmailForm(string modifyType, string tenantName)
        {
            var model = TenantService.GetTenantUserByName(tenantName);
            ViewBag.ModifyType = modifyType;
            return View("_ModifyPhoneAndEmailForm", model);
        }

        [Web.Extension.PermissionFilter("租户详情", "修改租户手机", "/Tenant/ModifyTenantUserContactPhone", "43377989-D92F-4166-BF5E-0894BB3D6315",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 3, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "43377989-D92F-4166-BF5E-0894BB3D6315")]
        public JsonResult ModifyTenantUserContactPhone(string tenantName, string phone)
        {
            return GetServiceJsonResult(() =>
            {
                var tenantUser = new TenantSimpleDTO
                {
                    TenantName = tenantName,
                    ContactPhone = phone
                };
                return TenantService.UpdateTenantUserBasicInfo(tenantUser);
            });
        }
        [Web.Extension.PermissionFilter("租户详情", "修改租户邮箱", "/Tenant/ModifyTenantUserContactPhone", "AEC0B6E2-B403-469B-A82A-5E135076EA36",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 3, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "AEC0B6E2-B403-469B-A82A-5E135076EA36")]
        public JsonResult ModifyTenantUserContactEmail(string tenantName, string email)
        {
            return GetServiceJsonResult(() =>
            {
                var tenantUser = new TenantSimpleDTO
                {
                    TenantName = tenantName,
                    ContactEmail = email
                };
                return TenantService.UpdateTenantUserBasicInfo(tenantUser);
            });
        }

        private void InitTenantApplicationHost(ref TenantUserDTO tenant)
        {
            if (!tenant.Applications.Any())
                return;
            foreach (var item in tenant.Applications)
            {
                try
                {
                    var app = GlobalConfig.Applications.FirstOrDefault(m => m.AppId == item.ApplicationId);
                    if(app != null)
                    {
                        var host = app.AppDomain.Replace(TenantConstant.SubDomain, tenant.TenantName).Replace("http://", "").Replace("https://", "").TrimEnd('/');
                        IPAddress[] IPs = Dns.GetHostAddresses(host);
                        item.DomainIp = IPs[0].ToString();
                    }
                }
                catch (Exception) { }
            }
        }
        #endregion

        #region 租户错误日志
        [Web.Extension.MenuFilter("租户管理", "租户错误日志",  "/Tenant/TenantErrorLog",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-file", AuthorityId = "98FC5170-C013-4BFA-A7FB-888BB570483E",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 3, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("租户错误日志", "租户错误日志", "/Tenant/TenantErrorLog", "98FC5170-C013-4BFA-A7FB-888BB570483E",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 4, IsPage = true, ResultType = ResultType.ActionResult, AuthorityId = "98FC5170-C013-4BFA-A7FB-888BB570483E")]
        public IActionResult TenantErrorlog(string id)
        {
            ViewBag.OpenServerTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnumWithAll<OpenServerType>();
            ViewBag.tenantuseropertionlog = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnumWithAll<OperationLogType>();
            return View();
        }

        public JsonResult LoadTenantErrorlog(int page = 1, int rows = 10, string tenantDisplayName = "",
            string order = "desc", int? openServerType = null)
        {
            var model = TenantService.FindOpenAppErrorLogsByFilter(page, rows, tenantDisplayName, openServerType, order);
            return Json(model);
        }

        public PartialViewResult GetTenantErrorlogForm(int id)
        {
            if (id != 0)
            {
                var result = TenantService.GetTenantErrorlogFormByProcessLogId(id);
                return PartialView("_tenantErrorLogForm", result);

            }

            return PartialView("_tenantErrorLogForm", null);
        }

        public JsonResult RollBackTenantDataBase(int id, int tenantId)
        {
            return GetServiceJsonResult(() =>
            {
                var message = TenantService.RollBackTenantDataBase(tenantId);
                if (!string.IsNullOrEmpty(message))
                    throw new ArgumentException(message);

                //更新数据库成功后，将日志状态更改为：成功
                return TenantService.UpdateTenantErrorLogStatus(id);
            });
        }

        public JsonResult ReSendOpenAppEmail(int id, int tenantId)
        {
            return GetServiceJsonResult(() =>
            {
                var tenant = TenantService.GetTenantUserById(tenantId);
                if (tenant == null)
                    throw new Exception("未找到相关的租户信息。");
                var result = TenantService.GetTenantErrorlogFormByProcessLogId(id);
                if (result == null)
                    throw new Exception("未找到相关的操作日志。");

                //数据来源于WorkRole或WindowsService：KC.WorkerRole.Jobs.OpenTenantAppsJob
                var errorMsg = result.Remark;
                var testInboxs = GlobalConfig.AdminEmails;
                var defaultSendTo = string.IsNullOrWhiteSpace(testInboxs)
                    ? new List<string>() { "tianchangjun@cfwin.com", "rayxuan@cfwin.com" }
                    : testInboxs.ArrayFromCommaDelimitedStrings().ToList();
                var emailTitle = "邮件标题：";
                var emailBody = "邮件内容：";

                var iTitleStart = errorMsg.IndexOf(emailTitle);
                var iBodyStart = errorMsg.IndexOf(emailBody);
                emailTitle = errorMsg.Substring(iTitleStart + emailTitle.Length, iBodyStart - iTitleStart - emailBody.Length);
                emailBody = errorMsg.Substring(iBodyStart + emailBody.Length);
                var mail = new EmailInfo
                {
                    UserId = RoleConstants.AdminUserId,
                    Tenant = tenant.TenantName,
                    EmailTitle = emailTitle.Replace("\r\n<br/>", string.Empty),
                    EmailBody = emailBody,
                    SendTo = !string.IsNullOrWhiteSpace(tenant.ContactEmail)
                        ? new List<string>() { tenant.ContactEmail }
                        : defaultSendTo
                };

                var isSucceed = new StorageQueueService().InsertEmailQueue(mail);
                //var isSucceed = (new TenantStorageQueueService(data)).InserEmailQueue(mail);
                //LogUtil.LogInfo((isSucceed ? "插入Email队列成功，邮件内容：" : "插入Email队列失败，邮件内容：") + emailBody);

                //var success = TenantService.SendTenantAppOpenEmail(data);
                //更新数据库成功后，将日志状态更改为：成功
                if (isSucceed)
                    return TenantService.UpdateTenantErrorLogStatus(id);

                return false;
            });
        }
        #endregion
    }

    public class ThreadParams
    {
        public int TenantId { get; set; }
        public string AppId { get; set; }
        public ITenantUserService TenantService { get; set; }
    }
}
