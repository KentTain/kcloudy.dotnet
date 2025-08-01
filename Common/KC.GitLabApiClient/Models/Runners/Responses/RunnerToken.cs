using Newtonsoft.Json;

namespace KC.GitLabApiClient.Models.Runners.Responses
{
    public sealed class RunnerToken
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
