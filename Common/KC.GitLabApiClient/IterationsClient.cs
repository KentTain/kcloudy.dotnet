using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KC.GitLabApiClient.Internal.Http;
using KC.GitLabApiClient.Internal.Paths;
using KC.GitLabApiClient.Internal.Queries;
using KC.GitLabApiClient.Models.Iterations.Requests;
using KC.GitLabApiClient.Models.Iterations.Responses;

namespace KC.GitLabApiClient
{
    internal class IterationsClient : IIterationsClient
    {
        private readonly GitLabHttpFacade _httpFacade;
        private readonly IterationsQueryBuilder _queryBuilder;

        public IterationsClient(GitLabHttpFacade httpFacade, IterationsQueryBuilder queryBuilder)
        {
            _httpFacade = httpFacade;
            _queryBuilder = queryBuilder;
        }

        public async Task<IList<Iteration>> GetAsync(ProjectId projectId = null, GroupId groupId = null,
            Action<IterationsQueryOptions> options = null)
        {
            var queryOptions = new IterationsQueryOptions();
            options?.Invoke(queryOptions);

            string path = "iterations";
            if (projectId != null)
            {
                path = $"projects/{projectId}/iterations";
            }
            else if (groupId != null)
            {
                path = $"groups/{groupId}/iterations";
            }

            string url = _queryBuilder.Build(path, queryOptions);
            return await _httpFacade.GetPagedList<Iteration>(url);
        }
    }
}
