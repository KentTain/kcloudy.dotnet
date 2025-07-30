using System.Runtime.Serialization;

namespace KC.GitLabApiClient.Models.Projects.Requests
{
    public enum ProjectVisibilityLevel
    {
        [EnumMember(Value = "private")]
        Private,
        [EnumMember(Value = "internal")]
        Internal,
        [EnumMember(Value = "public")]
        Public
    }
}
