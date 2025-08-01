using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KC.GitLabApiClient.Internal.Paths;
using KC.GitLabApiClient.Models.Job.Requests;
using KC.GitLabApiClient.Models.Job.Responses;
using KC.GitLabApiClient.Models.Pipelines.Requests;
using KC.GitLabApiClient.Models.Pipelines.Responses;

namespace KC.GitLabApiClient
{
    public interface IPipelineClient
    {
        Task<PipelineDetail> CancelAsync(ProjectId projectId, int pipelineId);
        Task<PipelineDetail> CreateAsync(ProjectId projectId, CreatePipelineRequest request);
        Task DeleteAsync(ProjectId projectId, int pipelineId);
        Task<IList<Pipeline>> GetAsync(ProjectId projectId, Action<PipelineQueryOptions> options = null);
        Task<PipelineDetail> GetAsync(ProjectId projectId, int pipelineId);
        Task<IList<Job>> GetJobsAsync(ProjectId projectId, int pipelineId, Action<JobQueryOptions> options = null);
        Task<IList<PipelineVariable>> GetVariablesAsync(ProjectId projectId, int pipelineId);
        Task<PipelineDetail> RetryAsync(ProjectId projectId, int pipelineId);
    }
}
