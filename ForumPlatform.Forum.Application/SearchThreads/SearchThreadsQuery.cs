using ForumPlatform.Forum.Application.Search;
using MediatR;

namespace ForumPlatform.Forum.Application.SearchThreads
{
	public record SearchThreadsQuery(string Text) : IRequest<List<ThreadSearchHit>>;
}
