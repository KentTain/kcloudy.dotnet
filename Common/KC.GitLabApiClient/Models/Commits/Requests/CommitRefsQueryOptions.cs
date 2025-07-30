using KC.GitLabApiClient.Models.Commits.Responses;

namespace KC.GitLabApiClient.Models.Commits.Requests
{
    /// <summary>
    ///     Query options for querying comit references
    /// </summary>
    public class CommitRefsQueryOptions
    {
        internal CommitRefsQueryOptions()
        {
        }

        /// <summary>
        ///     Queried commit type <see cref="CommitRefType" />
        /// </summary>
        public CommitRefType Type { get; set; }
    }
}
