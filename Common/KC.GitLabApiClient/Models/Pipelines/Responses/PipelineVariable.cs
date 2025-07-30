using Newtonsoft.Json;

namespace KC.GitLabApiClient.Models.Pipelines.Responses
{
    public class PipelineVariable
    {
        [JsonProperty("variable_type")]
        public PipelineVariableType VariableType { get; set; } = PipelineVariableType.Unknown;

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
