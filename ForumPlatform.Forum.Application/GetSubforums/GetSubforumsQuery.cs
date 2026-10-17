using MediatR;

namespace ForumPlatform.Forum.Application.GetSubforums
{
	public record GetSubforumsQuery() : IRequest<List<SubforumSummary>>;
	
	public record SubforumSummary(Guid Id, string Name, string Description, string Status);
}
