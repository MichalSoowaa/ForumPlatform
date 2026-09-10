using MediatR;

namespace ForumPlatform.Forum.Application.CreateThread
{
	public record CreateThreadCommand(
		string Title, string Body, Guid SubforumId, Guid AuthorId) : IRequest<CreateThreadResponse>;

	public record CreateThreadResponse(Guid ThreadId, string Title, Guid SubforumId);
}
