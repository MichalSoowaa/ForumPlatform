using ForumPlatform.Forum.Application.GetThreadDetail;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumPlatform.Forum.Application.Sorting
{
	/// <summary>
	/// Sorts the flat comment list of one thread. A single sort over the whole
	/// list is enough: the frontend groups it by depth and parent, which keeps
	/// input order, so top-level comments and the replies under each parent
	/// both come out in the chosen order.
	/// </summary>
	public static class CommentSorter
	{
		public static List<CommentDetail> Sort(List<CommentDetail> comments, SortMode sortMode)
		{
			var newestReplyByParent = comments
				.Where(c => c.ParentCommentId is not null)
				.GroupBy(c => c.ParentCommentId!.Value)
				.ToDictionary(g => g.Key, g => g.Max(c => c.CreatedAt));

			DateTime ActivityAt(CommentDetail c) =>
				newestReplyByParent.TryGetValue(c.Id, out var newest) && newest > c.CreatedAt
					? newest
					: c.CreatedAt;

			IOrderedEnumerable <CommentDetail> ordered = sortMode switch
			{
				SortMode.Quality => comments.OrderByDescending(c => c.FinalScore)
					.ThenByDescending(c => c.CreatedAt),

				SortMode.Freshness => comments.OrderByDescending(c => c.CreatedAt),

				SortMode.Activity => comments.OrderByDescending(c => ActivityAt(c)),

				SortMode.Expertise => comments.OrderByDescending(c => c.AuthorReputation)
					.ThenByDescending(c => c.FinalScore)
					.ThenByDescending(c => c.CreatedAt),
				_ => throw new ArgumentOutOfRangeException(nameof(sortMode), sortMode, null)
			};

			return ordered.ThenBy(c => c.Id).ToList();
		}
	}
}
