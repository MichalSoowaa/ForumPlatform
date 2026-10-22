using ForumPlatform.API.Auth;
using ForumPlatform.Forum.Application.Moderation.ApproveSubforum;
using ForumPlatform.Forum.Application.Moderation.GetPendingSubforums;
using ForumPlatform.Forum.Application.Moderation.RejectSubforum;
using MediatR;
using System.Security.Claims;

namespace ForumPlatform.API.Endpoints
{
	public record ApproveSubforumRequest(List<string>? Tags);
	public record RejectSubforumRequest(string? Reason);

	public static class ModerationEndpoints
	{
		public static void MapModerationEndpoints(this WebApplication app)
		{

			var group = app.MapGroup("/moderation").RequireAuthorization(AuthPolicies.Moderator);

			group.MapGet("/subforums/pending", async (IMediator mediator) =>
				Results.Ok(await mediator.Send(new GetPendingSubforumsQuery())))
				.WithName("GetPendingSubforums");

			group.MapPost("/subforums/{subforumId:guid}/approve",
				async (Guid subforumId, ApproveSubforumRequest request, ClaimsPrincipal user, IMediator mediator) =>
				{
					try
					{
						var command = new ApproveSubforumCommand(subforumId, user.GetUserId(), request.Tags ?? new List<string>());

						return Results.Ok(await mediator.Send(command));
					}
					catch (InvalidOperationException ex)
					{
						return Results.BadRequest(new { error = ex.Message });
					}
				})
				.WithName("ApproveSubforum");

			group.MapPost("/subforums/{subforumId:guid}/reject",
			async (Guid subforumId, RejectSubforumRequest request, ClaimsPrincipal user, IMediator mediator) =>
			{
				try
				{
					var command = new RejectSubforumCommand(subforumId, user.GetUserId(), request.Reason ?? "");
					return Results.Ok(await mediator.Send(command));
				}
				catch (InvalidOperationException ex)
				{
					return Results.BadRequest(new { error = ex.Message });
				}
			})
			.WithName("RejectSubforum");
		}
	}
}
