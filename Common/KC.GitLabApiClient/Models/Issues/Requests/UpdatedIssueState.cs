using System.Runtime.Serialization;

namespace KC.GitLabApiClient.Models.Issues.Requests
{
    public enum UpdatedIssueState
    {
        [EnumMember(Value = "close")]
        Close,

        [EnumMember(Value = "reopen")]
        Reopen
    }
}
