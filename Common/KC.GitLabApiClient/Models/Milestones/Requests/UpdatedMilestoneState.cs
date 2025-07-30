using System.Runtime.Serialization;

namespace KC.GitLabApiClient.Models.Milestones.Requests
{
    public enum UpdatedMilestoneState
    {
        [EnumMember(Value = "close")]
        Close,

        [EnumMember(Value = "activate")]
        Activate
    }
}
