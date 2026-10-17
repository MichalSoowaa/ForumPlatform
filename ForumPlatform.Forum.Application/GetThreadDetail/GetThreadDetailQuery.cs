using ForumPlatform.Forum.Application.Sorting;
using MediatR;

namespace ForumPlatform.Forum.Application.GetThreadDetail
{
	public record GetThreadDetailQuery(Guid ThreadId, SortMode Sort = SortMode.Quality) : IRequest<ThreadDetail?>;

	public record ThreadDetail(
		Guid Id,
		string Title,
		string Content,
		string Author,
		decimal AuthorReputation,
		decimal FinalScore,
		DateTime CreatedAt,
		List<CommentDetail> Comments
	);

	public record CommentDetail(
		Guid Id,
		string Content,
		string Author,
		decimal AuthorReputation,
		Guid? ParentCommentId,
		int Depth,
		decimal FinalScore,
		DateTime CreatedAt
	);
}
