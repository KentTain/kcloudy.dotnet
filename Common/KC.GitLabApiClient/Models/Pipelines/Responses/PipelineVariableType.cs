using System.Runtime.Serialization;

namespace KC.GitLabApiClient.Models.Pipelines.Responses
{
    public enum PipelineVariableType
    {
        Unknown,
        [EnumMember(Value = "env_var")]
        Environment,
        [EnumMember(Value = "file")]
        File
    }
}
