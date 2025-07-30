using System.Runtime.Serialization;

namespace KC.GitLabApiClient.Models.ToDoList.Responses
{
    public enum ToDoState
    {
        [EnumMember(Value = "pending")]
        Pending,

        [EnumMember(Value = "done")]
        Done,
    }
}
