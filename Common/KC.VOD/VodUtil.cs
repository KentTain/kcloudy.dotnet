using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.VOD.VodService;
using System;
using System.Collections.Generic;
using System.Text;

namespace KC.VOD
{

    public static class VodUtil
    {
        internal static IVodProvider GetProvider(Tenant tenant)
        {
            IVodProvider result = new AliyunVodProvider(tenant);
            var storageType = tenant.VodType;
            switch (storageType)
            {
                case VodType.File:
                    {
                        //result = new FileSystemProvider(tenant);
                    }
                    break;
                case VodType.Azure:
                    {
                        //result = new AzureProvider(tenant);
                    }
                    break;
                case VodType.AWS:
                    {
                        //result = new AliyunOSSProvider(tenant);
                    }
                    break;
                case VodType.Aliyun:
                    {
                        result = new AliyunVodProvider(tenant);
                    }
                    break;
                default:
                    {
                        result = new AliyunVodProvider(tenant);
                    }
                    break;
            }

            return result;
        }

        public static VodUploadSetting GetVideoSetting(Tenant tenant, string blobId)
        {
            var provider = GetProvider(tenant);
            return provider.GetVideoSetting(blobId);
        }

        public static VodUploadSetting RefreshVideoSetting(Tenant tenant, string videoId)
        {
            var provider = GetProvider(tenant);
            return provider.RefreshVideoSetting(videoId);
        }

        public static string UploadVideo(Tenant tenant, string blobId, string ext, byte[] blobData, Dictionary<string, string> metadata)
        {
            var provider = GetProvider(tenant);
            return provider.UploadVideo(blobId, ext, blobData, metadata);
        }

        public static List<string> GetVideoPlayAddress(Tenant tenant, string videoId)
        {
            var provider = GetProvider(tenant);
            return provider.GetVideoPlayAddress(videoId);
        }
    }
}
