using MediatR;

namespace ForumPlatform.Users.Application.Login
{
	public record LoginQuery(string Email, string Password) : IRequest<LoginResponse>;

	public record LoginResponse(Guid UserId, string Email, string Username, string Token);
}
