using MediatR;
using ForumPlatform.Forum.Infrastructure.Persistance;
using ForumPlatform.Users.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using ForumPlatform.Forum.Application.Sorting;

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

			var authors = await _userDb.Users
				.Where(u => authorIds.Contains(u.Id))
				.ToDictionaryAsync(u => u.Id, u => new AuthorInfo(u.UserName ?? "Unknown", u.TotalReputation), cancellationToken);

			var threadSummaries = threads.Select(t => new ThreadSummary(
				t.Id,
				t.Title,
				authors.GetValueOrDefault(t.AuthorId, AuthorInfo.Unknown).Username,
				authors.GetValueOrDefault(t.AuthorId, AuthorInfo.Unknown).Reputation,
				t.CommentCount,
				t.FinalScore,
				t.CreatedAt,
				t.LastActivityAt
			)).ToList();

			return ThreadSorter.Sort(threadSummaries, request.Sort);
		}
	}
}
