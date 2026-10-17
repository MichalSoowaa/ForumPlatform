using ForumPlatform.Users.Infrastructure.Persistance;
using ForumPlatform.Forum.Infrastructure.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ForumPlatform.Forum.Domain.Entities;
using ForumPlatform.Forum.Application.Sorting;

namespace ForumPlatform.Forum.Application.GetThreadDetail
{
	public class GetThreadDetailQueryHandler : IRequestHandler<GetThreadDetailQuery, ThreadDetail?>
	{
		private readonly ForumDbContext _forumDb;
		private readonly UserDbContext _userDb;

		public GetThreadDetailQueryHandler(ForumDbContext forumDb, UserDbContext userDb)
		{
			_forumDb = forumDb;
			_userDb = userDb;
		}

		public async Task<ThreadDetail?> Handle(GetThreadDetailQuery request, CancellationToken cancellationToken)
		{
			var thread = await _forumDb.Threads.FirstOrDefaultAsync(t => t.Id == request.ThreadId, cancellationToken);

			if (thread == null)
				return null;
			
			var comments = await _forumDb.Comments
				.Where(c => c.ThreadId == request.ThreadId)
				.OrderBy(c => c.CreatedAt)
				.Select(c => new
				{
					c.Id,
					c.Body,
					c.AuthorId,
					c.ParentCommentId,
					c.Depth,
					c.FinalScore,
					c.CreatedAt
				})
				.ToListAsync(cancellationToken);

			var authorIds = comments.Select(c => c.AuthorId).Append(thread.AuthorId).Distinct().ToList();

			var usernamesById = await _userDb.Users
				.Where(u => authorIds.Contains(u.Id))
				.ToDictionaryAsync(
					u => u.Id, u => new AuthorInfo(u.UserName ?? "Unknown", u.TotalReputation), cancellationToken);

			var commentDetails = comments.Select(c =>
			{
				var author = usernamesById.GetValueOrDefault(c.AuthorId, AuthorInfo.Unknown);
				
				return new CommentDetail(
					c.Id,
					c.Body,
					author.Username,
					author.Reputation,
					c.ParentCommentId,
					c.Depth,
					c.FinalScore,
					c.CreatedAt
				);
			}).ToList();

			var threadAuthor = usernamesById.GetValueOrDefault(thread.AuthorId, AuthorInfo.Unknown);

			return new ThreadDetail(
				thread.Id,
				thread.Title,
				thread.Body,
				threadAuthor.Username,
				threadAuthor.Reputation,
				thread.FinalScore,
				thread.CreatedAt,
				CommentSorter.Sort(commentDetails, request.Sort)
			);
		}
	}
}
