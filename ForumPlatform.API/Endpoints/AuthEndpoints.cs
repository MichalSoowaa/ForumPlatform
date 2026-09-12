using ForumPlatform.Users.Application.Login;
using ForumPlatform.Users.Application.Register;
using MediatR;

namespace ForumPlatform.API.Endpoints
{
	public static class AuthEndpoints
	{
		public static void MapAuthEndpoints(this WebApplication app)
		{
			app.MapPost("/auth/register", async (RegisterUserCommand command, IMediator mediator) =>
			{
				try
				{
					var result = await mediator.Send(command);
					return Results.Created("/auth/register", result);
				}
				catch (InvalidOperationException ex)
				{
					return Results.BadRequest(new { error = ex.Message });
				}
			})
				.WithName("RegisterUser")
				.Produces(StatusCodes.Status201Created)
				.Produces(StatusCodes.Status400BadRequest);

			app.MapPost("/auth/login", async (LoginQuery query, IMediator mediator) =>
			{
				try
				{
					var result = await mediator.Send(query);
					return Results.Ok(result);
				}
				catch (UnauthorizedAccessException)
				{
					return Results.Unauthorized();
				}
			})
				.WithName("LoginUser")
				.Produces(StatusCodes.Status200OK)
				.Produces(StatusCodes.Status401Unauthorized);
		}
	}
}
