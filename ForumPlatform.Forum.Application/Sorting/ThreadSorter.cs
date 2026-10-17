using ForumPlatform.Forum.Application.GetThreadsBySubforum;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumPlatform.Forum.Application.Sorting
{
	public static class ThreadSorter
	{
		// Every mode ends with an Id tie-breaker. Many items share a score (the
		// 0.5 prior, equal heuristics), and without it ties would come back in
		// whatever order the database happened to return them.
		public static List<ThreadSummary> Sort(IEnumerable<ThreadSummary> threads, SortMode sortMode)
		{
			IOrderedEnumerable<ThreadSummary> ordered = sortMode switch
			{
				SortMode.Quality => threads.OrderByDescending(t => t.FinalScore)
					.ThenByDescending(t => t.CreatedAt),

				SortMode.Freshness => threads.OrderByDescending(t => t.CreatedAt),

				SortMode.Activity => threads.OrderByDescending(t => t.LastActivityAt)
					.ThenByDescending(t => t.CommentCount),

				SortMode.Expertise => threads.OrderByDescending(t => t.AuthorReputation)
					.ThenByDescending(t => t.FinalScore)
					.ThenByDescending(t => t.CreatedAt),

				_ => throw new ArgumentOutOfRangeException(nameof(sortMode), sortMode, null)
			};

			return ordered.ThenBy(t => t.Id).ToList();
		}
	}
}
