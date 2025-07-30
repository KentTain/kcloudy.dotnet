using Newtonsoft.Json;

namespace KC.GitLabApiClient.Models.Projects.Responses
{
    public sealed class Permissions
    {
        [JsonProperty("group_access")]
        public Access GroupAccess { get; set; }

        [JsonProperty("project_access")]
        public Access ProjectAccess { get; set; }
    }
}
