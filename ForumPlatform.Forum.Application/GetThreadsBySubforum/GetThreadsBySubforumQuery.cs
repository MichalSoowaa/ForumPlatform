using ForumPlatform.Forum.Application.Sorting;
using MediatR;

namespace ForumPlatform.Forum.Application.GetThreadsBySubforum
{
	public record GetThreadsBySubforumQuery(Guid SubforumId, SortMode Sort = SortMode.Activity, Guid? ViewerId = null) : IRequest<List<ThreadSummary>>;

	/// <param name="NewCommentCount">
	/// Comments by other users since the viewer last opened the thread.
	/// Null when the viewer has never opened it (or is not logged in).
	/// </param>
	public record ThreadSummary(
		Guid Id,
		string Title,
		string Author,
		decimal AuthorReputation,
		int CommentCount,
		decimal FinalScore,
		DateTime CreatedAt,
		DateTime LastActivityAt,
		int? NewCommentCount
	);
}
