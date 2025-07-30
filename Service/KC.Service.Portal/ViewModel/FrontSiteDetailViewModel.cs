using KC.Service.DTO.Portal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KC.Service.ViewModel.Portal
{
    public class FrontSiteDetailViewModel<T>
    {
        public FrontSiteDetailViewModel()
        {
            RecommendInfos = new List<RecommendInfoDTO>();
        }
        public T DetailInfo { get; set; }

        public List<RecommendInfoDTO> RecommendInfos { get; set; }
    }
}
