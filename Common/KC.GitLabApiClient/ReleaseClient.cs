using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KC.GitLabApiClient.Internal.Http;
using KC.GitLabApiClient.Internal.Paths;
using KC.GitLabApiClient.Internal.Queries;
using KC.GitLabApiClient.Internal.Utilities;
using KC.GitLabApiClient.Models.Projects.Responses;
using KC.GitLabApiClient.Models.Releases.Requests;
using KC.GitLabApiClient.Models.Releases.Responses;

namespace KC.GitLabApiClient
{
    public sealed class ReleaseClient : IReleaseClient
    {
        private readonly GitLabHttpFacade _httpFacade;
        private readonly ReleaseQueryBuilder _releaseQueryBuilder;

        internal ReleaseClient(
            GitLabHttpFacade httpFacade,
            ReleaseQueryBuilder releaseQueryBuilder)
        {
            _httpFacade = httpFacade;
            _releaseQueryBuilder = releaseQueryBuilder;
        }

        /// <summary>
        /// Retrieves a release by its name
        /// </summary>
        /// <param name="projectId">The ID, path or <see cref="Project"/> of the project.</param>
        /// <param name="tagName">The release, you want to retrieve.</param>
        /// <returns></returns>
        public async Task<Release> GetAsync(ProjectId projectId, string tagName) =>
            await _httpFacade.Get<Release>($"projects/{projectId}/releases/{tagName}");

        /// <summary>
        /// Get a list of releases
        /// </summary>
        /// <param name="projectId">The ID, path or <see cref="Project"/> of the project.</param>
        /// <param name="options">Query options <see cref="ReleaseQueryOptions"/>.</param>
        /// <returns></returns>
        public async Task<IList<Release>> GetAsync(ProjectId projectId, Action<ReleaseQueryOptions> options = null)
        {
            var queryOptions = new ReleaseQueryOptions();
            options?.Invoke(queryOptions);

            string url = _releaseQueryBuilder.Build($"projects/{projectId}/releases", queryOptions);
            return await _httpFacade.GetPagedList<Release>(url);
        }

        /// <summary>
        /// Create a release
        /// </summary>
        /// <param name="projectId">The ID, path or <see cref="Project"/> of the project.</param>
        /// <param name="request">Create release request.</param>
        /// <returns></returns>
        public async Task<Release> CreateAsync(ProjectId projectId, CreateReleaseRequest request) =>
            await _httpFacade.Post<Release>($"projects/{projectId}/releases", request);

        /// <summary>
        /// Update a release
        /// </summary>
        /// <param name="projectId">The ID, path or <see cref="Project"/> of the project.</param>
        /// <param name="tagName">The tag name of the release, you want to update.</param>
        /// <param name="request">Update release request</param>
        /// <returns></returns>
        public async Task<Release> UpdateAsync(ProjectId projectId, string tagName, UpdateReleaseRequest request) =>
            await _httpFacade.Put<Release>($"projects/{projectId}/releases/{tagName.UrlEncode()}", request);

        /// <summary>
        /// Delete a release
        /// </summary>
        /// <param name="projectId">The ID, path or <see cref="Project"/> of the project.</param>
        /// <param name="tagName">The tag name of the release, you want to delete.</param>
        /// <returns></returns>
        public async Task DeleteAsync(ProjectId projectId, string tagName) =>
            await _httpFacade.Delete($"projects/{projectId}/releases/{tagName.UrlEncode()}");
    }
}
