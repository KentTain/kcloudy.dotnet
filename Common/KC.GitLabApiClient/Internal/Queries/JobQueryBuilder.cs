using KC.GitLabApiClient.Internal.Utilities;
using KC.GitLabApiClient.Models.Job.Requests;

namespace KC.GitLabApiClient.Internal.Queries
{
    internal sealed class JobQueryBuilder : QueryBuilder<JobQueryOptions>
    {
        #region Overrides of QueryBuilder<PipelineQueryOptions>

        /// <inheritdoc />
        protected override void BuildCore(Query query, JobQueryOptions options)
        {
            if (options.Scope != JobScope.All)
            {
                query.Add("scope", options.Scope.ToLowerCaseString());
            }
        }

        #endregion
    }
}
