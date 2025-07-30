using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KC.GitLabApiClient.Internal.Http;
using KC.GitLabApiClient.Internal.Paths;
using KC.GitLabApiClient.Internal.Queries;
using KC.GitLabApiClient.Models.Trees.Requests;
using KC.GitLabApiClient.Models.Trees.Responses;

namespace KC.GitLabApiClient
{
    public sealed class TreesClient : ITreesClient
    {
        private readonly GitLabHttpFacade _httpFacade;
        private readonly TreeQueryBuilder _treeQueryBuilder;

        internal TreesClient(GitLabHttpFacade httpFacade, TreeQueryBuilder treeQueryBuilder)
        {
            _httpFacade = httpFacade;
            _treeQueryBuilder = treeQueryBuilder;
        }

        public async Task<IList<Tree>> GetAsync(ProjectId projectId, Action<TreeQueryOptions> options = null)
        {
            var queryOptions = new TreeQueryOptions();
            options?.Invoke(queryOptions);

            string url = _treeQueryBuilder.Build($"projects/{projectId}/repository/tree", queryOptions);
            return await _httpFacade.GetPagedList<Tree>(url);
        }
    }
}
