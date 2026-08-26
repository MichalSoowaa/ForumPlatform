using MediatR;

namespace ForumPlatform.Users.Application.Register
{
	public record RegisterUserCommand(string Email, string Username, string Password, string PasswordConfirm) : IRequest<RegisterUserResponse>;

	public record RegisterUserResponse(Guid UserId, string Email, string Username, string Token);
}
