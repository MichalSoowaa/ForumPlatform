using ForumPlatform.API.Auth;
using ForumPlatform.Forum.Application.CreateComment;
using ForumPlatform.Forum.Application.CreateSubforum;
using ForumPlatform.Forum.Application.CreateThread;
using MediatR;
using System.Security.Claims;

namespace ForumPlatform.API.Endpoints
{
	// Request DTOs - the actual JSON shape client sends.
	// Defined alongside the endpoints, since these are wire-contract concerns specific to this API,
	// not domain/application types.
	public record CreateSubforumRequest(string Name, string Description);
	public record CreateThreadRequest(string Title, string Body, Guid SubforumId);
	public record CreateCommentRequest(string Body, Guid ThreadId, Guid? ParentCommentId);

	public static class ForumEndpoints
	{
		public static void MapForumEndpoints(this WebApplication app)
		{
			app.MapPost("/subforums",
				async (CreateSubforumRequest request, ClaimsPrincipal user, IMediator mediator) =>
			{
				var command = new CreateSubforumCommand(request.Name, request.Description, user.GetUserId());

				try
				{
					var result = await mediator.Send(command);
					return Results.Created($"/subforums/{result.SubforumId}", result);
				}
				catch (InvalidOperationException e)
				{
					return Results.BadRequest(new { error = e.Message });
				}
			})
				.WithName("CreateSubforum")
				.RequireAuthorization()
				.Produces(StatusCodes.Status201Created)
				.Produces(StatusCodes.Status400BadRequest);

			app.MapPost("/threads", async (CreateThreadRequest request, ClaimsPrincipal user, IMediator mediator) =>
			{
				var command = new CreateThreadCommand(request.Title, request.Body, request.SubforumId, user.GetUserId());

				try
				{
					var result = await mediator.Send(command);
					return Results.Created($"/threads/{result.ThreadId}", result);
				}
				catch (InvalidOperationException ex)
				{
					return Results.BadRequest(new { error = ex.Message });
				}
			})
			.WithName("CreateThread")
			.RequireAuthorization()
			.Produces(StatusCodes.Status201Created)
			.Produces(StatusCodes.Status400BadRequest);

			app.MapPost("/comments", async (CreateCommentRequest request, ClaimsPrincipal user, IMediator mediator) =>
			{
				var command = new CreateCommentCommand(
					request.Body, request.ThreadId, user.GetUserId(), request.ParentCommentId);

				try
				{
					var result = await mediator.Send(command);
					return Results.Created($"/comments/{result.CommentId}", result);
				}
				catch (InvalidOperationException ex)
				{
					return Results.BadRequest(new { error = ex.Message });
				}
			})
				.WithName("CreateComment")
				.RequireAuthorization()
				.Produces(StatusCodes.Status201Created)
				.Produces(StatusCodes.Status400BadRequest);
		}
	}
}
