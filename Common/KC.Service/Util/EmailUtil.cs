using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Extension;
using KC.Framework.Util;
using KC.Service.Base;
using KC.Service.WebApiService.Business;
using KC.Framework.Base;

namespace KC.Service.Util
{
    public static class EmailUtil
    {
        public const int OutlookConfigId = 9;//Outlook邮件配置
        public static bool Send(EmailConfig config, string subject, string body, List<string> mails, List<string> cc = null)
        {
            try
            {
                if (mails == null || !mails.Any())
                {
                    return false;
                    throw new ArgumentNullException("mails", "没有接收信息的电子邮件");
                }

                if (string.IsNullOrWhiteSpace(body))
                {
                    return false;
                    throw new ArgumentNullException("body", "没有信息内容");
                }

                if (config == null)
                {
                    return false;
                    throw new ArgumentNullException("config", "邮件服务未配置或未启用");
                }

                var mailTo = mails.Select(u => new MailAddress(u, u)).ToList();
                //邮件
                var mailObj = new MailMessage
                {
                    From = new MailAddress(config.Account, "财富共赢业务平台"),
                    Subject = AppnendTitle(subject),
                    Body = body,
                    IsBodyHtml = true,
                    BodyEncoding = Encoding.UTF8
                };

                foreach (var item in mailTo)
                {
                    mailObj.To.Add(item); //收件人邮箱地址
                }

                if (cc != null && cc.Any())
                {
                    foreach (var item in cc)
                    {
                        mailObj.CC.Add(item);
                    }
                }

                //发送参数
                var smtp = new SmtpClient
                {
                    Host = config.Server,
                    Port = config.Port,
                    EnableSsl = config.EnableSsl,
                    UseDefaultCredentials = true,
                    Credentials = new NetworkCredential(config.Account, config.Password)
                };

                smtp.Send(mailObj);

                return true;
            }
            catch (System.Net.Mail.SmtpFailedRecipientsException ex)
            {
                LogUtil.LogError(
                    string.Format(
                        "邮件发送失败，未能发送邮件给【{0}】，" + Environment.NewLine +
                        "邮件标题为：{1}，" + Environment.NewLine + 
                        "邮件内容为：{2}，" + Environment.NewLine +
                        "错误消息为：{3}",
                        mails.ToCommaSeparatedString(), subject, body, ex.Message));
                return false;
            }
            catch (System.Net.Mail.SmtpException ex)
            {
                LogUtil.LogError(
                    string.Format(
                        "连接到 SMTP 服务器失败，未能发送邮件给【{0}】，" + Environment.NewLine +
                        "邮件标题为：{1}，" + Environment.NewLine + 
                        "邮件内容为：{2}，" + Environment.NewLine +
                        "错误消息为：{3}",
                        mails.ToCommaSeparatedString(), subject, body, ex.Message));
                return false;
            }
            catch (Exception ex)
            {
                LogUtil.LogError(string.Format("其他异常，未能发送邮件给【{0}】，" + Environment.NewLine +
                        "邮件标题为：{1}，" + Environment.NewLine +
                        "邮件内容为：{2}，" + Environment.NewLine +
                        "错误消息为：{3}",
                    mails.ToCommaSeparatedString(), subject, body, ex.Message), ex.StackTrace);

                return false;
            }
        }
        public static Task<bool> SendAsync(EmailConfig config, string subject, string body, List<string> mails, List<string> cc = null)
        {
            try
            {
                if (mails == null || !mails.Any())
                {
                    return Task.FromResult(false);
                    throw new ArgumentNullException("mails", "没有接收信息的电子邮件");
                }

                if (string.IsNullOrWhiteSpace(body))
                {
                    return Task.FromResult(false);
                    throw new ArgumentNullException("body", "没有信息内容");
                }

                if (config == null)
                {
                    return Task.FromResult(false);
                    throw new ArgumentNullException("config", "邮件服务未配置或未启用");
                }

                var mailTo = mails.Select(u => new MailAddress(u, u)).ToList();
                //邮件
                var mailObj = new MailMessage
                {
                    From = new MailAddress(config.Account, "财富共赢业务平台"),
                    Subject = AppnendTitle(subject),
                    Body = body,
                    IsBodyHtml = true,
                    BodyEncoding = Encoding.UTF8
                };

                foreach (var item in mailTo)
                {
                    mailObj.To.Add(item); //收件人邮箱地址
                }

                if (cc != null && cc.Any())
                {
                    foreach (var item in cc)
                    {
                        mailObj.CC.Add(item);
                    }
                }

                //发送参数
                var smtp = new SmtpClient
                {
                    Host = config.Server,
                    Port = config.Port,
                    EnableSsl = config.EnableSsl,
                    UseDefaultCredentials = true,
                    Credentials = new NetworkCredential(config.Account, config.Password)
                };

                smtp.SendMailAsync(mailObj);

                return Task.FromResult(true);
            }
            catch (System.Net.Mail.SmtpFailedRecipientsException ex)
            {
                LogUtil.LogError(
                    string.Format(
                        "邮件发送失败，未能发送邮件给【{0}】，" + Environment.NewLine +
                        "邮件标题为：{1}，" + Environment.NewLine +
                        "邮件内容为：{2}，" + Environment.NewLine +
                        "错误消息为：{3}",
                        mails.ToCommaSeparatedString(), subject, body, ex.Message));
                return Task.FromResult(false);
            }
            catch (System.Net.Mail.SmtpException ex)
            {
                LogUtil.LogError(
                    string.Format(
                        "连接到 SMTP 服务器失败，未能发送邮件给【{0}】，" + Environment.NewLine + 
                        "邮件标题为：{1}，" + Environment.NewLine +
                        "邮件内容为：{2}，" + Environment.NewLine +
                        "错误消息为：{3}",
                        mails.ToCommaSeparatedString(), subject, body, ex.Message));
                return Task.FromResult(false);
            }
            catch (Exception ex)
            {
                LogUtil.LogError(string.Format("其他异常，未能发送邮件给【{0}】，" + Environment.NewLine +
                        "邮件标题为：{1}，" + Environment.NewLine +
                        "邮件内容为：{2}，" + Environment.NewLine +
                        "错误消息为：{3}",
                    mails.ToCommaSeparatedString(), subject, body, ex.Message), ex.StackTrace);

                return Task.FromResult(false);
            }
        }

        public static void SendAdministratorMail(EmailConfig config, string subject, string body)
        {
            var testInboxs = GlobalConfig.AdminEmails;
            var defaultSendTo = string.IsNullOrWhiteSpace(testInboxs)
                ? new List<string>() { "tianchangjun@cfwin.com", "rayxuan@cfwin.com" }
                : testInboxs.ArrayFromCommaDelimitedStrings().ToList();

            Send(config, subject, body, defaultSendTo, null);

            //var ccmailList = new List<string> { GlobalDataBase.CustomerServiceGroup };
            //Send(config, subject, body, mailList, ccmailList);
        }
        public static void SendAdministratorMail(string subject, string body)
        {
            try
            {
                var config = new GlobalConfigApiService(null).GetDefaultEmailConfig();
                SendAdministratorMail(config, subject, body);
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
            }
            
        }

        private static string AppnendTitle(string subject)
        {
            var sysType = GlobalConfig.SystemType;
            switch (sysType)
            {
                case SystemType.Dev:
                    subject = "[开发环境]：" + subject;
                    break;
                case SystemType.Test:
                    subject = "[测试环境]：" + subject;
                    break;
                case SystemType.Beta:
                    subject = "[演示环境]：" + subject;
                    break;
                default:
                    break;
            }

            return subject;
        }
    }
}
