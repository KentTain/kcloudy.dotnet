using KC.GitLabApiClient.Models.Branches.Requests;

namespace KC.GitLabApiClient.Internal.Queries
{
    internal class BranchQueryBuilder : QueryBuilder<BranchQueryOptions>
    {
        protected override void BuildCore(Query query, BranchQueryOptions options)
        {
            if (!string.IsNullOrEmpty(options.Search))
                query.Add("search", options.Search);
        }
    }
}
