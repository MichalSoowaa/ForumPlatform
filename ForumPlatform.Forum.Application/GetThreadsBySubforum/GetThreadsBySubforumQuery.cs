using ForumPlatform.Forum.Application.Sorting;
using MediatR;

namespace ForumPlatform.Forum.Application.GetThreadsBySubforum
{
	public record GetThreadsBySubforumQuery(Guid SubforumId, SortMode Sort = SortMode.Activity) : IRequest<List<ThreadSummary>>;

	public record ThreadSummary(
		Guid Id,
		string Title,
		string Author,
		decimal AuthorReputation,
		int CommentCount,
		decimal FinalScore,
		DateTime CreatedAt,
		DateTime LastActivityAt
	);
}
