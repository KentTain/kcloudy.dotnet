using System.Threading.Tasks;
using KC.GitLabApiClient.Internal.Paths;
using KC.GitLabApiClient.Models.Files.Responses;

namespace KC.GitLabApiClient
{
    public interface IFilesClient
    {
        Task<File> GetAsync(ProjectId projectId, string filePath, string reference = "master");
    }
}
