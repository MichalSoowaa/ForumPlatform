using MediatR;
using ForumPlatform.Forum.Infrastructure.Persistance;
using ForumPlatform.Users.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using ForumPlatform.Forum.Application.Sorting;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;

namespace ForumPlatform.Forum.Application.GetThreadsBySubforum
{
	/// <summary>
	/// Joins across ForumDbContext and UserDbContext in application code rather than at the SQL level
	/// - the two live in separate schemas owned by different modules,
	/// so a single EF query spanning both isn't available.
	/// Two queries plus an in-memory dictionary join.
	/// </summary>
	public class GetThreadsBySubforumQueryHandler : IRequestHandler<GetThreadsBySubforumQuery, List<ThreadSummary>>
	{
		private readonly ForumDbContext _forumDb;
		private readonly UserDbContext _userDb;

		public GetThreadsBySubforumQueryHandler(ForumDbContext forumDb, UserDbContext userDb)
		{
			_forumDb = forumDb;
			_userDb = userDb;
		}

		public async Task<List<ThreadSummary>> Handle(GetThreadsBySubforumQuery request, CancellationToken cancellationToken)
		{
			var threads = await _forumDb.Threads
				.Where(t => t.SubforumId == request.SubforumId)
				.OrderByDescending(t => t.LastActivityAt)
				.Select(t => new
				{
					t.Id,
					t.Title,
					t.AuthorId,
					t.CommentCount,
					t.FinalScore,
					t.CreatedAt,
					t.LastActivityAt
				})
				.ToListAsync(cancellationToken);

			var authorIds = threads.Select(t => t.AuthorId).Distinct().ToList();

			var authorsById = await _userDb.Users
				.Where(u => authorIds.Contains(u.Id))
				.ToDictionaryAsync(u => u.Id, u => new AuthorInfo(u.UserName ?? "Unknown", u.TotalReputation), cancellationToken);

			var newCounts = await LoadNewCommentCountsAsync(
				request.ViewerId, threads.Select(t => t.Id).ToList(), cancellationToken);

			var threadSummaries = threads.Select(t =>
			{
				int? newCount = request.ViewerId is null
					? null
					: newCounts.TryGetValue(t.Id, out var n) ? (int?)n : null;

				return new ThreadSummary(
					t.Id,
					t.Title,
					authorsById.GetValueOrDefault(t.AuthorId, AuthorInfo.Unknown).Username,
					authorsById.GetValueOrDefault(t.AuthorId, AuthorInfo.Unknown).Reputation,
					t.CommentCount,
					t.FinalScore,
					t.CreatedAt,
					t.LastActivityAt,
					newCount
				);
			});

			return ThreadSorter.Sort(threadSummaries, request.Sort);
		}

		private async Task<Dictionary<Guid, int>> LoadNewCommentCountsAsync(
			Guid? viewerId, List<Guid> threadIds, CancellationToken ct)
		{
			var result = new Dictionary<Guid, int>();

			if (viewerId is null || threadIds.Count == 0)
				return result;

			var userId = viewerId.Value;

			var visited = await _forumDb.ThreadVisits
				.Where(v => v.UserId == userId && threadIds.Contains(v.ThreadId))
				.Select(v => v.ThreadId)
				.ToListAsync(ct);

			foreach (var id in visited)
				result[id] = 0;

			// The viewer's own comments never count as "new", and the global query filter
			// already excludes deleted comments.
			var counts = await _forumDb.Comments
				.Where(c => threadIds.Contains(c.ThreadId) && c.AuthorId != userId)
				.Join(_forumDb.ThreadVisits.Where(v => v.UserId == userId),
					c => c.ThreadId, v => v.ThreadId, (c, v) => new { c.ThreadId, c.CreatedAt, v.LastVisitedAt })
				.Where(x => x.CreatedAt > x.LastVisitedAt)
				.GroupBy(x => x.ThreadId)
				.Select(g => new { ThreadId = g.Key, Count = g.Count() })
				.ToListAsync(ct);

			foreach (var c in counts)
				result[c.ThreadId] = c.Count;

			return result;
		}
	}
}
