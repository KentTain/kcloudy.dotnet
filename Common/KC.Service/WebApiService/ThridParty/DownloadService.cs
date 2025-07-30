using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Service.Base;

namespace KC.Service.WebApiService.ThridParty
{
    public class DownloadService : OAuth2ClientRequestBase
    {
        protected const string ServiceName = "KC.Service.WebApiService.ThridParty.DownloadService";
        public DownloadService(
            System.Net.Http.IHttpClientFactory httpClient,
            Microsoft.Extensions.Logging.ILogger<DownloadService> logger)
            : base(httpClient, logger)
        {
        }

        protected override OAuth2ClientInfo GetOAuth2ClientInfo()
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResult<byte[]>> DownloadFileFromUrlAsync(string url)
        {
            ServiceResult<byte[]> result = null;
            await DownLoadFile(
                ServiceName + ".DownloadFileFromUrl",
                url,
                callback =>
                {
                    result = new ServiceResult<byte[]>(ServiceResultType.Success, string.Empty, callback);
                },
                errorMessage =>
                {
                    result = new ServiceResult<byte[]>(ServiceResultType.Error, errorMessage);
                });

            return result;
        }

        public async Task<ServiceResult<List<ShjCallLog>>> GetShjCallLogListAsync(CallConfig config)
        {
            var downloadUrl = config.SmsUrl + "interface/api/?action=getmsg";
            ServiceResult<List<ShjCallLog>> result = null;
            await WebSendGetAsync<ShjResult>(
                ServiceName + ".GetShjCallLogList",
                downloadUrl,
                ApplicationConstant.AccScope,
                callback =>
                {
                    if (callback.error == 0 && callback.data != null)
                    {
                        var data = callback.data.msg.Where(m => m.Event.Equals("cdr", StringComparison.OrdinalIgnoreCase)).ToList();
                        result = new ServiceResult<List<ShjCallLog>>(ServiceResultType.Success, string.Empty, data);
                    } 
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<ShjCallLog>>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                false);

            return result;
        }

        public async Task<ServiceResult<byte[]>> DownloadShjVoiceAsync(CallConfig config, string fileName)
        {
            var downloadUrl = config.SmsUrl.EndWithSlash() + "admin/?m=interface&c=api&a=record_download&filename=" + config.Value1 + fileName;
            var fileBytes = await GetDownloadFileBytes(ServiceName + ".DownloadShjVoice", downloadUrl);

            return fileBytes != null ? 
                new ServiceResult<byte[]>(ServiceResultType.Success, string.Empty, fileBytes) 
                : new ServiceResult<byte[]>(ServiceResultType.Error, "下载文件为空。");

            //ServiceResult<byte[]> result = null;
            //DownLoadFile(
            //    ServiceName + ".DownloadShjVoice",
            //    downloadUrl,
            //    callback =>
            //    {
            //        result = new ServiceResult<byte[]>(ServiceResultType.Success, string.Empty, callback);
            //    },
            //    errorMessage =>
            //    {
            //        result = new ServiceResult<byte[]>(ServiceResultType.Error, httpStatusCode, errorMessage);
            //    });

            //return result;
        }

        
    }
}
