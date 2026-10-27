using ForumPlatform.Forum.Application.FindSimilarThreads;
using ForumPlatform.Forum.Application.SearchThreads;
using MediatR;

namespace ForumPlatform.API.Endpoints
{
	public record FindSimilarThreadsRequest(string? Title, string? Body);

	public static class SearchEndpoints
	{
		public static void MapSearchEndpoints(this WebApplication app)
		{
			// Both require login: every call costs one embedding request to an external API
			// that has quotas, so they shouldn't be open to anonymous traffic.
			app.MapGet("/search", async (string? q, IMediator mediator) =>
				Results.Ok(await mediator.Send(new SearchThreadsQuery(q ?? ""))))
				.WithName("SearchThreads")
				.RequireAuthorization();

			app.MapPost("/threads/similar", async (FindSimilarThreadsRequest request, IMediator mediator) =>
				Results.Ok(await mediator.Send(new FindSimilarThreadsQuery(request.Title ?? "", request.Body ?? ""))))
				.WithName("FindSimilarThreads")
				.RequireAuthorization();
		}
	}
}
