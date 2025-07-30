using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Model.Job.Table;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using KC.Service.Job;

namespace KC.WebApi.Job.Controllers
{
    [Authorize]
    public class JobApiController : Web.Controllers.WebApiBaseController
    {
        private IJobDbService _jobService => ServiceProvider.GetService<IJobDbService>();
        public JobApiController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<JobApiController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        /// <summary>
        /// 获取QueryError数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("LoadPagenatedQueueErrors")]
        public PaginatedBase<QueueErrorMessage> LoadPagenatedQueueErrors(int page = 1, int rows = 10, string queueName = null)
        {
            return _jobService.FindPagenatedQueueErrorList(page, rows, queueName, "CreatedDate", "desc");
        }

        /// <summary>
        /// 删除QueryError数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("RemoveByRowKey")]
        public bool RemoveByRowKey(string rowKey)
        {
            return _jobService.RemoveByRowKey(rowKey);
        }

    }
}
