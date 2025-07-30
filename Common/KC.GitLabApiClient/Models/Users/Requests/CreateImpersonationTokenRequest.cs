using KC.GitLabApiClient.Internal.Paths;
using KC.GitLabApiClient.Internal.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KC.GitLabApiClient.Models.Users.Requests
{
    /// <summary>
    /// Creates a new Impersonation Token. 
    /// Note only administrators can create new Impersonation Token.   /// </summary>
    public sealed class CreateImpersonationTokenRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateImpersonationTokenRequest"/> class.
        /// <param name="userId">UserId.</param>
        /// <param name="name">Name.</param>
        /// <param name="expiresAt">User expiresAt.</param>
        /// </summary>
        public CreateImpersonationTokenRequest(UserId userId, string name, DateTime? expiresAt, List<Responses.Scope> scopes)
        {
            Guard.NotEmpty(name, nameof(name));
            UserId = userId;
            Name = name;
            Scopes = scopes;
            if (expiresAt.HasValue)
                ExpiresAt = expiresAt.Value;
        }

        [JsonProperty("user_id")]
        public UserId UserId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("scopes[]")]
        public List<Responses.Scope> Scopes { get; } = new List<Responses.Scope>();

        [JsonProperty("expires_at")]
        public DateTime ExpiresAt { get; set; }
    }
}
