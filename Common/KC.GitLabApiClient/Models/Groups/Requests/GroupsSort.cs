using System.Runtime.Serialization;

namespace KC.GitLabApiClient.Models.Groups.Requests
{
    public enum GroupsSort
    {
        [EnumMember(Value = "asc")]
        Ascending,

        [EnumMember(Value = "desc")]
        Descending
    }
}
