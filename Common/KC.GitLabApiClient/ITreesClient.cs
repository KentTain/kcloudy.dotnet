using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KC.GitLabApiClient.Internal.Paths;
using KC.GitLabApiClient.Models.Trees.Requests;
using KC.GitLabApiClient.Models.Trees.Responses;

namespace KC.GitLabApiClient
{
    public interface ITreesClient
    {
        Task<IList<Tree>> GetAsync(ProjectId projectId, Action<TreeQueryOptions> options = null);
    }
}
