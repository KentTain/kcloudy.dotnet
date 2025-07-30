using System.Threading.Tasks;
using KC.GitLabApiClient.Internal.Http;
using KC.GitLabApiClient.Internal.Paths;
using KC.GitLabApiClient.Internal.Utilities;
using KC.GitLabApiClient.Models.Files.Responses;

namespace KC.GitLabApiClient
{
    public sealed class FilesClient : IFilesClient
    {
        private readonly GitLabHttpFacade _httpFacade;

        internal FilesClient(GitLabHttpFacade httpFacade) => _httpFacade = httpFacade;

        public async Task<File> GetAsync(ProjectId projectId, string filePath, string reference = "master")
        {
            return await _httpFacade.Get<File>($"projects/{projectId}/repository/files/{filePath.UrlEncode()}?ref={reference}");
        }
    }
}
