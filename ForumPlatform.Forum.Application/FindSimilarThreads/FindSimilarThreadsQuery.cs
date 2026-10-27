using ForumPlatform.Forum.Application.Search;
using MediatR;

namespace ForumPlatform.Forum.Application.FindSimilarThreads
{
	public record FindSimilarThreadsQuery(string Title, string Body) : IRequest<List<ThreadSearchHit>>;
}
