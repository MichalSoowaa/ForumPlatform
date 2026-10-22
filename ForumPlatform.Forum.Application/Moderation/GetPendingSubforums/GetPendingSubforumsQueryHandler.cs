using ForumPlatform.Forum.Infrastructure.Persistance;
using ForumPlatform.Users.Infrastructure.Persistance;
using MediatR;
using ForumPlatform.Forum.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ForumPlatform.Forum.Application.Moderation.GetPendingSubforums
{
	public class GetPendingSubforumsQueryHandler : IRequestHandler<GetPendingSubforumsQuery, List<PendingSubforum>>
	{
		private readonly ForumDbContext _forumDb;
		private readonly UserDbContext _userDb;

		public GetPendingSubforumsQueryHandler(ForumDbContext forumDb, UserDbContext userDb)
		{
			_forumDb = forumDb;
			_userDb = userDb;
		}

		public async Task<List<PendingSubforum>> Handle(GetPendingSubforumsQuery request, CancellationToken cancellationToken)
		{
			var pending = await _forumDb.Subforums
				.Where(s => s.Status == SubforumStatus.Pending)
				.OrderBy(s => s.CreatedAt)
				.Select(s => new { s.Id, s.Name, s.Description, s.CreatedByUserId, s.CreatedAt })
				.ToListAsync(cancellationToken);

			var proposerIds = pending.Select(p => p.CreatedByUserId).Distinct().ToList();

			var usernames = await _userDb.Users
				.Where(u => proposerIds.Contains(u.Id))
				.ToDictionaryAsync(u => u.Id, u => u.UserName ?? "unknown", cancellationToken);

			return pending
				.Select(p => new PendingSubforum(
					p.Id, p.Name, p.Description, usernames.GetValueOrDefault(p.CreatedByUserId, "unknown"), p.CreatedAt))
				.ToList();
		}
	}
}
