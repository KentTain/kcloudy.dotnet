using System.Runtime.Serialization;

namespace KC.GitLabApiClient.Models.Pipelines.Requests
{
    public enum PipelineOrder
    {
        [EnumMember(Value = "id")]
        Id,
        [EnumMember(Value = "status")]
        Status,
        [EnumMember(Value = "ref")]
        Ref,
        [EnumMember(Value = "user_id")]
        UserId
    }
}
