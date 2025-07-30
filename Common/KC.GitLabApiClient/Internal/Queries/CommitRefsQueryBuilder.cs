using KC.GitLabApiClient.Internal.Utilities;
using KC.GitLabApiClient.Models.Commits.Requests;
using KC.GitLabApiClient.Models.Commits.Responses;

namespace KC.GitLabApiClient.Internal.Queries
{
    internal sealed class CommitRefsQueryBuilder : QueryBuilder<CommitRefsQueryOptions>
    {
        protected override void BuildCore(Query query, CommitRefsQueryOptions options)
        {
            if (options.Type != CommitRefType.All)
            {
                query.Add("type", options.Type.ToLowerCaseString());
            }
        }
    }
}
