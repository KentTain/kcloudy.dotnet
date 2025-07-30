using System.Threading.Tasks;
using KC.GitLabApiClient.Models.Markdown.Request;
using KC.GitLabApiClient.Models.Markdown.Response;

namespace KC.GitLabApiClient
{
    public interface IMarkdownClient
    {
        Task<Markdown> RenderAsync(RenderMarkdownRequest request);
    }
}
