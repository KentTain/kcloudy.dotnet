using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using KC.GitLabApiClient.Internal.Http;
using KC.GitLabApiClient.Internal.Paths;
using KC.GitLabApiClient.Internal.Utilities;
using KC.GitLabApiClient.Models.Projects.Responses;
using KC.GitLabApiClient.Models.Users.Requests;
using KC.GitLabApiClient.Models.Users.Responses;
using Newtonsoft.Json.Linq;

namespace KC.GitLabApiClient
{
    /// <summary>
    /// Used to query GitLab API to retrieve, modify, create users.
    /// <exception cref="GitLabException">Thrown if request to GitLab API fails</exception>
    /// <exception cref="HttpRequestException">Thrown if request to GitLab API fails</exception>
    /// </summary>
    public sealed class UsersClient : IUsersClient
    {
        private readonly GitLabHttpFacade _httpFacade;

        internal UsersClient(GitLabHttpFacade httpFacade) =>
            _httpFacade = httpFacade;

        /// <summary>
        /// Retrieves registered users.
        /// </summary>
        public async Task<IList<User>> GetAsync() =>
            await _httpFacade.GetPagedList<User>("users");

        /// <summary>
        /// Retrieves an user matched by name.
        /// </summary>
        /// <param name="name">Username of the user.</param>
        /// <returns>User or NULL if it was not found.</returns>
        public async Task<User> GetAsync(string name)
        {
            Guard.NotEmpty(name, nameof(name));
            return (await _httpFacade.Get<IList<User>>($"users?username={name}")).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves users by filter.
        /// </summary>
        /// <param name="filter">Filter used for usernames and emails.</param>
        /// <returns>Users list satisfying the filter.</returns>
        public async Task<IList<User>> GetByFilterAsync(string filter)
        {
            Guard.NotEmpty(filter, nameof(filter));
            return await _httpFacade.GetPagedList<User>($"users?search={filter}");
        }

        /// <summary>
        /// Creates new user
        /// </summary>
        /// <param name="request">Request to create user.</param>
        /// <returns>Newly created user.</returns>
        public async Task<User> CreateAsync(CreateUserRequest request) =>
            await _httpFacade.Post<User>("users", request);

        /// <summary>
        /// Updates existing user
        /// </summary>
        /// <param name="userId">Id of the user.</param>
        /// <param name="request">Request to update user.</param>
        /// <returns>Newly modified user.</returns>
        public async Task<User> UpdateAsync(UserId userId, UpdateUserRequest request) =>
            await _httpFacade.Put<User>($"users/{userId}", request);

        /// <summary>
        /// Retrieves current, authenticated user session.
        /// </summary>
        /// <returns>Session of authenticated user.</returns>
        public async Task<Session> GetCurrentSessionAsync() =>
            await _httpFacade.Get<Session>("user");

        /// <summary>
        /// Deletes user.
        /// </summary>
        /// <param name="userId">Id of the user.</param>
        public async Task DeleteAsync(UserId userId) =>
            await _httpFacade.Delete($"users/{userId}");

        /// <summary>
        /// Get the projects list of a user.
        /// </summary>
        /// <param name="userId">The ID, path or <see cref="User"/> of the project.</param>
        public async Task<IList<Project>> GetProjectsAsync(UserId userId) =>
            await _httpFacade.Get<IList<Project>>($"users/{userId}/projects");

        /// <summary>
        /// Creates new user
        /// </summary>
        /// <param name="request">Request to create user token.</param>
        /// <returns>Newly created user.</returns>
        public async Task<ImpersonationToken> CreateImpersonationTokenAsync(UserId userId, CreateImpersonationTokenRequest request)
        {
            var keyValues = new List<KeyValuePair<string, string>>();
            keyValues.Add(new KeyValuePair<string, string>("name", request.Name));
            keyValues.Add(new KeyValuePair<string, string>("expires_at", request.ExpiresAt.ToString()));
            for (int i = 0; i < request.Scopes.Count; i++)
            {
                keyValues.Add(new KeyValuePair<string, string>("scopes[]", request.Scopes[i].ToLowerCaseString()));
            }

            return await _httpFacade.Post<ImpersonationToken>($"users/{userId}/impersonation_tokens", keyValues);
        }

        /// <summary>
        /// Retrieves an ImpersonationToken matched by userId & tokenId.
        /// </summary>
        /// <param name="userId">userId of the user.</param>
        /// <returns>ImpersonationToken or NULL if it was not found.</returns>
        public async Task<IList<ImpersonationToken>> GetAllImpersonationTokensAsync(UserId userId) =>
            await _httpFacade.Get<IList<ImpersonationToken>>($"users/{userId}/impersonation_tokens");

        /// <summary>
        /// Retrieves an ImpersonationToken matched by userId & tokenId.
        /// </summary>
        /// <param name="userId">userId of the user.</param>
        /// <param name="state">state of the ImpersonationToken.</param>
        /// <returns>ImpersonationToken or NULL if it was not found.</returns>
        public async Task<IList<ImpersonationToken>> GetAllImpersonationTokensAsync(UserId userId, ImpersonationState state) =>
            await _httpFacade.Get<IList<ImpersonationToken>>($"users/{userId}/impersonation_tokens?state={state.ToLowerCaseString()}");

        /// <summary>
        /// Retrieves an ImpersonationToken matched by userId & tokenId.
        /// </summary>
        /// <param name="userId">userId of the user.</param>
        /// <param name="tokenId">tokenId of the ImpersonationToken.</param>
        /// <returns>ImpersonationToken or NULL if it was not found.</returns>
        public async Task<ImpersonationToken> GetImpersonationTokenAsync(UserId userId, ImpersonationTokenId tokenId) =>
            (await _httpFacade.Get<IList<ImpersonationToken>>($"users/{userId}/impersonation_tokens/{tokenId}")).FirstOrDefault();

        /// <summary>
        /// Delete an ImpersonationToken matched by userId & tokenId.
        /// </summary>
        /// <param name="userId">userId of the user.</param>
        /// <param name="tokenId">tokenId of the ImpersonationToken.</param>
        /// <returns>ImpersonationToken or NULL if it was not found.</returns>
        public async Task RevokImpersonationTokenAsync(UserId userId, ImpersonationTokenId tokenId) =>
            await _httpFacade.Delete($"users/{userId}/impersonation_tokens/{tokenId}");

    }
}
