using System.Runtime.Serialization;

namespace KC.GitLabApiClient.Models.Projects.Responses
{
    public enum ExportStatusEnum
    {
        [EnumMember(Value = "none")]
        None,
        [EnumMember(Value = "started")]
        Started,
        [EnumMember(Value = "after_export_action")]
        AfterExportAction,
        [EnumMember(Value = "finished")]
        Finished
    }
}
