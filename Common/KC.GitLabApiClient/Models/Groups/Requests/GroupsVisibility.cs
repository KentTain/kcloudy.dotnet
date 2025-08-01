using System.Runtime.Serialization;

namespace KC.GitLabApiClient.Models.Groups.Requests
{
    public enum GroupsVisibility
    {
        [EnumMember(Value = "public")]
        Public,

        [EnumMember(Value = "internal")]
        Internal,

        [EnumMember(Value = "private")]
        Private
    }
}
