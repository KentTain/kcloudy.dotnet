using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace KC.GitLabApiClient.Models.Users.Responses
{
    public sealed class ImpersonationToken
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("user_id")]
        public int UserId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("revoked")]
        public bool Revoked { get; set; }

        [JsonProperty("scopes")]
        public List<Scope> Scopes { get; } = new List<Scope>();

        [JsonProperty("impersonation")]
        public bool Impersonation { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("expires_at")]
        public DateTime ExpiresAt { get; set; }

    }

    public enum Scope
    {
        API,
        READ_USER,
        READ_REPOSITORY,
        WRITE_REPOSITORY,
        READ_REGISTRY,
        SUDO
    }

    public enum ImpersonationState
    {
        ALL,
        ACTIVE,
        INACTIVE
    }
}
