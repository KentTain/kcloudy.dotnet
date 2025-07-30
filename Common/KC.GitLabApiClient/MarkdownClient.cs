using System.Threading.Tasks;
using KC.GitLabApiClient.Internal.Http;
using KC.GitLabApiClient.Models.Markdown.Request;
using KC.GitLabApiClient.Models.Markdown.Response;

namespace KC.GitLabApiClient
{
    /// <summary>
    /// Used to render a markdown document.
    /// </summary>
    public sealed class MarkdownClient : IMarkdownClient
    {
        private readonly GitLabHttpFacade _httpFacade;

        internal MarkdownClient(GitLabHttpFacade httpFacade) =>
            _httpFacade = httpFacade;

        public async Task<Markdown> RenderAsync(RenderMarkdownRequest request) =>
            await _httpFacade.Post<Markdown>($"markdown", request);
    }
}
