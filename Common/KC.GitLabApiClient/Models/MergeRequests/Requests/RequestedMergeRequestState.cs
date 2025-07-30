using System.Runtime.Serialization;

namespace KC.GitLabApiClient.Models.MergeRequests.Requests
{
    public enum RequestedMergeRequestState
    {
        [EnumMember(Value = "close")]
        Close,
        [EnumMember(Value = "reopen")]
        Reopen
    }
}
