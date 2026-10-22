using MediatR;

namespace ForumPlatform.Forum.Application.Moderation.GetPendingSubforums
{
	public record GetPendingSubforumsQuery() : IRequest<List<PendingSubforum>>;

	public record PendingSubforum(Guid Id, string Name, string Description, string ProposedByUsername, DateTime ProposedAt);
}
