using Newtonsoft.Json;

namespace KC.GitLabApiClient.Models.Issues.Responses
{
    public class IssueTaskCompletionStatus
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("completed_count")]
        public int Completed { get; set; }
    }
}
