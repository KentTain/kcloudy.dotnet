using KC.Framework.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace KC.VOD
{
    public interface IVodProvider
    {
        VodUploadSetting GetVideoSetting(string blobId);

        VodUploadSetting RefreshVideoSetting(string videoId);

        string UploadVideo(string blobId, string ext, byte[] blobData, Dictionary<string, string> metadata);

        List<string> GetVideoPlayAddress(string videoId);
    }


    public abstract class VodProviderBase : IVodProvider
    {
        public abstract VodUploadSetting GetVideoSetting(string blobId);

        public abstract VodUploadSetting RefreshVideoSetting(string voidId);

        public abstract string UploadVideo(string blobId, string ext, byte[] blobData, Dictionary<string, string> metadata);

        public abstract List<string> GetVideoPlayAddress(string videoId);
    }
}
