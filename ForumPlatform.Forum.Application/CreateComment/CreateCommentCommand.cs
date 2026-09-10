using MediatR;

namespace ForumPlatform.Forum.Application.CreateComment
{
	public record CreateCommentCommand(
		string Body, Guid ThreadId, Guid AuthorId, Guid? ParentCommentId) : IRequest<CreateCommandResponse>;

	public record CreateCommandResponse(Guid CommentId, Guid ThreadId, Guid? ParentCommentId, int Depth);
}
