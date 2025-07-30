using System.Collections.Generic;
using System.Threading.Tasks;
using KC.GitLabApiClient.Internal.Paths;
using KC.GitLabApiClient.Models.Projects.Responses;
using KC.GitLabApiClient.Models.Webhooks.Requests;
using KC.GitLabApiClient.Models.Webhooks.Responses;

namespace KC.GitLabApiClient
{
    public interface IWebhookClient
    {
        /// <summary>
        /// Retrieves a hook by its id
        /// </summary>
        /// <param name="projectId">The ID, path or <see cref="Project"/> of the project.</param>
        /// <param name="hookId">The hook ID, you want to retrieve.</param>
        /// <returns></returns>
        Task<Webhook> GetAsync(ProjectId projectId, long hookId);

        /// <summary>
        /// Retrieves all project hooks
        /// </summary>
        /// <param name="projectId">The ID, path or <see cref="Project"/> of the project.</param>
        /// <returns></returns>
        Task<IList<Webhook>> GetAsync(ProjectId projectId);

        /// <summary>
        /// Create new webhook
        /// </summary>
        /// <param name="projectId">The ID, path or <see cref="Project"/> of the project.</param>
        /// <param name="request">Create hook request.</param>
        /// <returns>newly created hook</returns>
        Task<Webhook> CreateAsync(ProjectId projectId, CreateWebhookRequest request);

        /// <summary>
        /// Delete a webhook
        /// </summary>
        /// <param name="projectId">The ID, path or <see cref="Project"/> of the project.</param>
        /// <param name="hookId">The hook ID, you want to delete.</param>
        Task DeleteAsync(ProjectId projectId, long hookId);

        /// <summary>
        /// Update new webhook
        /// </summary>
        /// <param name="projectId">The ID, path or <see cref="Project"/> of the project.</param>
        /// <param name="hookId">The hook ID, you want to update.</param>
        /// <param name="request">Create hook request.</param>
        /// <returns>newly created hook</returns>
        Task<Webhook> UpdateAsync(ProjectId projectId, long hookId, CreateWebhookRequest request);
    }
}
