using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Framework.Tenant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using KC.Service.Doc;
using KC.Enums.Doc;
using KC.Service;
using KC.Service.DTO.Doc;
using KC.Service.DTO;

namespace KC.WebApi.Doc.Controllers
{
    /// <summary>
    /// 文档管理
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    public class DocumentApiController : Web.Controllers.WebApiBaseController
    {
        private IDocumentInfoService _docService => ServiceProvider.GetService<IDocumentInfoService>();
        
        public DocumentApiController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<DocumentApiController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        [HttpGet]
        [Route("FindDocuments")]
        public ServiceResult<PaginatedBaseDTO<DocumentInfoDTO>> FindDocuments(int page, int rows, int? cateid, LableType type, string name)
        {
            return GetServiceResult(() =>
            {
                return _docService.LoadPaginatedDocumentsByFilter(page, rows, cateid, name, null, null, null);
            });
        }

        /// <summary>
        /// 根据Id，获取配置对象
        /// </summary>
        /// <param name="id">配置Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetDocumentById")]
        public ServiceResult<DocumentInfoDTO> GetDocumentById(int id)
        {
            return GetServiceResult(() =>
            {
                return _docService.GetDocumentById(id);
            });
        }
    }
}
