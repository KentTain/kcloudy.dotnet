using System;
using System.Collections.Generic;
using KC.Framework.Util;
using KC.Framework.Tenant;
using KC.Model.Component.Queue;
using KC.Service.Events;
using KC.Service.DTO.Account;
using KC.Service.Component;

namespace KC.Service.Account.EventHandlers
{
    public class UserCreatedHandler : IHandler
    {
        private UserDTO _user;
        private string _pwd;
        private Tenant _tenant;

        public UserCreatedHandler(Tenant tenant, UserDTO user, string password)
        {
            _user = user;
            _pwd = password;
            _tenant = tenant;
        }

        public bool Handle()
        {
            LogUtil.LogInfo("------------UserCreatedHandler---------");

            try
            {
                //var emailBody = "您好，{0}！<br/>    您在公司（{1}）内部系统的登录用户已经创建成功，登录初始密码为: {2}, 请登录系统后马上更改密码。<br/>如遇问题请您与公司系统管理员联系。";
                var emailBody =
                    @"<!doctype html><html lang='en'><head><meta charset='UTF-8'><title></title><style>*,html,body{{margin:0;padding:0}}.divdiv{{width:638px;position:relative;margin:0 auto;background:#17212E;padding:20px;padding-top:0}}table{{width:100%;background:#454d5b;text-align:center;margin:0 auto;margin-top:50px;margin-bottom:50px;border-collapse:collapse}}.p_white{{color:#fff}}table tr td:first-child{{text-align:left;width:33%}}table tr td{{text-align:left;padding:9px;color:#fff;min-width:136px}}table tr th{{color:#fff;padding:13px;background:#4b5761;text-align:left}}table tr td p{{word-break:break-all;color:inherit}}.app_tb tr td:first-child{{text-align:left}}tr{{border:1px solid #636363}}.tips tr{{border:none}}.tips tr td:first-child{{text-align:left}}a{{text-decoration:none;color:#fff;word-break:break-all}}.guanggao_contain{{width:100%;position:relative;float:left;margin-top:30px;margin-bottom:30px}}.guanggao_logo{{float:left;margin-left:20px;display:block;position:relative;background-position:35px 14px;color:#afafaf}}.guanggao_logo_p{{float:left;margin-left:20px;display:block;position:relative;background-position:35px 14px;color:#afafaf;margin-bottom:10px;clear:both;width:100%}}.tips{{min-height:50px;background:0 0;font-size:12px}}.guanggao_logo:hover{{color:#fff}}.title p{{ font-size: 16px; color: #c6d4df; font-family: Arial, Helvetica, sans-serif; font-weight: bold;margin: 8px;
}}</style></head><body><div class='divdiv'><div class='guanggao_contain'><a href='http://www.cfwin.com/' target='_blank' class='guanggao_logo'>
<img src='data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAIwAAAAyCAYAAACOADM7AAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAyJpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuMy1jMDExIDY2LjE0NTY2MSwgMjAxMi8wMi8wNi0xNDo1NjoyNyAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvIiB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIgeG1sbnM6c3RSZWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZVJlZiMiIHhtcDpDcmVhdG9yVG9vbD0iQWRvYmUgUGhvdG9zaG9wIENTNiAoV2luZG93cykiIHhtcE1NOkluc3RhbmNlSUQ9InhtcC5paWQ6OEM5OUYyMUZBMUE0MTFFN0FFNThCQjEyNDI1MjA0MEIiIHhtcE1NOkRvY3VtZW50SUQ9InhtcC5kaWQ6OEM5OUYyMjBBMUE0MTFFN0FFNThCQjEyNDI1MjA0MEIiPiA8eG1wTU06RGVyaXZlZEZyb20gc3RSZWY6aW5zdGFuY2VJRD0ieG1wLmlpZDo4Qzk5RjIxREExQTQxMUU3QUU1OEJCMTI0MjUyMDQwQiIgc3RSZWY6ZG9jdW1lbnRJRD0ieG1wLmRpZDo4Qzk5RjIxRUExQTQxMUU3QUU1OEJCMTI0MjUyMDQwQiIvPiA8L3JkZjpEZXNjcmlwdGlvbj4gPC9yZGY6UkRGPiA8L3g6eG1wbWV0YT4gPD94cGFja2V0IGVuZD0iciI/PsljtMkAAAmlSURBVHja7F0JbBVV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'></a></div>
<div class='title'><p>主题：【产融协作员工账号创建成功-财富共赢】</p><p>{0}员工{1},您好！</p><p>您公司的系统管理员为您添加的产融协作员工账号已创建成功。</p> </div>
<table><tr><td><p>公司名称:</p></td><td><p>{0}</p></td></tr>
<tr><td><p>租户代码:</p></td><td><p>{2}</p></td></tr>
<tr><td><p>账号：</p></td><td><p>{3}</p></td></tr>
<tr><td><p>初始密码：</p></td><td><p>{4}</p></td></tr>
</table>
<div style='padding-top:12px' colspan='2'><span style='font-size: 17px; color: #c6d4df; font-family: Arial, Helvetica, sans-serif; font-weight: bold;'><p>登陆地址：<a href='http://sso.cfwin.com' target='_blank'>http://sso.cfwin.com</a>。请登录后立即修改密码，如遇问题，请与公司系统管理员联系。</p></span></div><div class='clear'></div><div class='tips'><p class='guanggao_logo_p'>友情链接</p>
<a href='http://www.cfwin.com/' target='_blank' class='guanggao_logo'>财富共赢金融超市</a> <a href='http://www.starlu.com/' target='_blank' class='guanggao_logo'>大陆之星赊销商城</a> <a href='http://www.cfwinclub.com/' target='_blank' class='guanggao_logo'>财富共赢俱乐部</a></div></div></body></html>";
                var mail = new EmailInfo
                {
                    Tenant = _tenant.TenantName,
                    UserId = _user.UserId,
                    EmailTitle = "创建登陆用户成功",
                    EmailBody = string.Format(emailBody,_tenant.TenantDisplayName, _user.DisplayName, _tenant.TenantName,_user.UserName, _pwd),
                    SendTo = new List<string>() { _user.Email }
                };
                new TenantStorageQueueService(_tenant).InsertEmailQueue(mail);

                return true;
            }
            catch (Exception ex)
            {
                LogUtil.LogError("创建用户成功后，处理后续业务出错：", ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
            }
        }
    }


}
