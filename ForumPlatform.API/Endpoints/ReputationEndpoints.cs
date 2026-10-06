using ForumPlatform.Reputation.Application.CastVote;
using ForumPlatform.Reputation.Domain.Entities;
using MediatR;
using System.Security.Claims;
using ForumPlatform.API.Auth;

namespace ForumPlatform.API.Endpoints
{
	public static class ReputationEndpoints
	{
		public static void MapReputationEndpoints(this WebApplication app)
		{
			app.MapPost("/votes", async (CastVoteRequest request, ClaimsPrincipal user, IMediator mediator) =>
			{
				var command = new CastVoteCommand(user.GetUserId(), request.ContentId, request.ContentType, request.Value);
				var result = await mediator.Send(command);

				return Results.Ok(result);
			})
				.WithName("CastVote")
				.RequireAuthorization()
				.Produces<CastVoteResponse>(StatusCodes.Status200OK)
				.Produces(StatusCodes.Status400BadRequest);
		}

		public record CastVoteRequest(Guid ContentId, QualityContentType ContentType, VoteValue Value);
	}
}
