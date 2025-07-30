using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KC.GitLabApiClient.Internal.Paths;
using KC.GitLabApiClient.Models.Iterations.Requests;
using KC.GitLabApiClient.Models.Iterations.Responses;

namespace KC.GitLabApiClient
{
    public interface IIterationsClient
    {
        Task<IList<Iteration>> GetAsync(ProjectId projectId = null, GroupId groupId = null,
            Action<IterationsQueryOptions> options = null);
    }
}
