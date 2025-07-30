using System.Collections.Generic;
using System.Threading.Tasks;
using KC.GitLabApiClient.Internal.Paths;
using KC.GitLabApiClient.Models.Projects.Responses;
using KC.GitLabApiClient.Models.Users.Requests;
using KC.GitLabApiClient.Models.Users.Responses;

namespace KC.GitLabApiClient
{
    public interface IUsersClient
    {
        /// <summary>
        /// Retrieves registered users.
        /// </summary>
        Task<IList<User>> GetAsync();

        /// <summary>
        /// Retrieves an user matched by name.
        /// </summary>
        /// <param name="name">Username of the user.</param>
        /// <returns>User or NULL if it was not found.</returns>
        Task<User> GetAsync(string name);

        /// <summary>
        /// Retrieves users by filter.
        /// </summary>
        /// <param name="filter">Filter used for usernames and emails.</param>
        /// <returns>Users list satisfying the filter.</returns>
        Task<IList<User>> GetByFilterAsync(string filter);

        /// <summary>
        /// Creates new user
        /// </summary>
        /// <param name="request">Request to create user.</param>
        /// <returns>Newly created user.</returns>
        Task<User> CreateAsync(CreateUserRequest request);

        /// <summary>
        /// Updates existing user
        /// </summary>
        /// <param name="userId">Id of the user.</param>
        /// <param name="request">Request to update user.</param>
        /// <returns>Newly modified user.</returns>
        Task<User> UpdateAsync(UserId userId, UpdateUserRequest request);

        /// <summary>
        /// Retrieves current, authenticated user session.
        /// </summary>
        /// <returns>Session of authenticated user.</returns>
        Task<Session> GetCurrentSessionAsync();

        /// <summary>
        /// Deletes user.
        /// </summary>
        /// <param name="userId">Id of the user.</param>
        Task DeleteAsync(UserId userId);

        /// <summary>
        /// Get the projects list of a user.
        /// </summary>
        /// <param name="userId">The ID, path or <see cref="User"/> of the project.</param>
        Task<IList<Project>> GetProjectsAsync(UserId userId);

        /// <summary>
        /// Creates new Impersonation Token
        /// </summary>
        /// <param name="request">Request to create Impersonation Token.</param>
        /// <returns>Newly created Impersonation Token.</returns>
        Task<ImpersonationToken> CreateImpersonationTokenAsync(UserId userId, CreateImpersonationTokenRequest request);

        /// <summary>
        /// Retrieves an ImpersonationToken matched by userId & tokenId.
        /// </summary>
        /// <param name="userId">userId of the user.</param>
        /// <returns>ImpersonationToken or NULL if it was not found.</returns>
        Task<IList<ImpersonationToken>> GetAllImpersonationTokensAsync(UserId userId);

        /// <summary>
        /// Retrieves an ImpersonationToken matched by userId & tokenId.
        /// </summary>
        /// <param name="userId">userId of the user.</param>
        /// <param name="state">state of the ImpersonationToken.</param>
        /// <returns>ImpersonationToken or NULL if it was not found.</returns>
        Task<IList<ImpersonationToken>> GetAllImpersonationTokensAsync(UserId userId, ImpersonationState state);

        /// <summary>
        /// Retrieves an ImpersonationToken matched by userId & tokenId.
        /// </summary>
        /// <param name="userId">userId of the user.</param>
        /// <param name="tokenId">tokenId of the ImpersonationToken.</param>
        /// <returns>ImpersonationToken or NULL if it was not found.</returns>
        Task<ImpersonationToken> GetImpersonationTokenAsync(UserId userId, ImpersonationTokenId tokenId);

        /// <summary>
        /// Delete an ImpersonationToken matched by userId & tokenId.
        /// </summary>
        /// <param name="userId">userId of the user.</param>
        /// <param name="tokenId">tokenId of the ImpersonationToken.</param>
        /// <returns>ImpersonationToken or NULL if it was not found.</returns>
        Task RevokImpersonationTokenAsync(UserId userId, ImpersonationTokenId tokenId);
    }
}
