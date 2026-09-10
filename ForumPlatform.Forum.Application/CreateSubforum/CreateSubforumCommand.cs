using MediatR;

namespace ForumPlatform.Forum.Application.CreateSubforum
{
	public record CreateSubforumCommand(string Name, string Description, Guid CreatedByUserId) : IRequest<CreateSubforumResponse>;

	public record CreateSubforumResponse(Guid SubforumId, string Name, string Status);
}
