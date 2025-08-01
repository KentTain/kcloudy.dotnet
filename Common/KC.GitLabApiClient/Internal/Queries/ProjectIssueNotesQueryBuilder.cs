using System;
using KC.GitLabApiClient.Models;
using KC.GitLabApiClient.Models.Notes.Requests;

namespace KC.GitLabApiClient.Internal.Queries
{
    internal class ProjectIssueNotesQueryBuilder : QueryBuilder<IssueNotesQueryOptions>
    {
        protected override void BuildCore(Query query, IssueNotesQueryOptions options)
        {
            if (options.SortOrder != SortOrder.Descending)
                query.Add("sort", GetSortOrderQueryValue(options.SortOrder));

            if (options.Order != NoteOrder.CreatedAt)
                query.Add("order_by", GetNoteOrderQueryValue(options.Order));
        }

        private static string GetNoteOrderQueryValue(NoteOrder order)
        {
            switch (order)
            {
                case NoteOrder.CreatedAt:
                    return "created_at";
                case NoteOrder.UpdatedAt:
                    return "updated_at";
                default:
                    throw new NotSupportedException($"Order {order} is not supported");
            }
        }
    }
}
