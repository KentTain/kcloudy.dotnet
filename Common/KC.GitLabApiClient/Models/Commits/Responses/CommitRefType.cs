using System.Runtime.Serialization;

namespace KC.GitLabApiClient.Models.Commits.Responses
{
    public enum CommitRefType
    {
        [EnumMember(Value = "all")]
        All,
        [EnumMember(Value = "branch")]
        Branch,
        [EnumMember(Value = "tag")]
        Tag
    }
}
