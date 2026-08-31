using MediatR;
using Microsoft.AspNetCore.Identity;
using ForumPlatform.Users.Application.Abstraction;
using ForumPlatform.Users.Domain.Entities.User;

namespace ForumPlatform.Users.Application.Login
{
	public class LoginQueryHandler : IRequestHandler<LoginQuery, LoginResponse>
	{
		private readonly UserManager<User> _userManager;
		private readonly ITokenService _tokenService;

		public LoginQueryHandler(UserManager<User> userManager, ITokenService tokenService)
		{
			_userManager = userManager;
			_tokenService = tokenService;
		}

		public async Task<LoginResponse> Handle(LoginQuery request, CancellationToken cancellationToken)
		{
			var user = await _userManager.FindByEmailAsync(request.Email);

			if (user == null)
			{
				throw new UnauthorizedAccessException($"User with email {request.Email} not found.");
			}

			var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);

			if (!passwordValid)
			{
				throw new Exception("Invalid email or password.");
			}

			var token = _tokenService.GenerateToken(user.Id, user.Email, user.UserName);
			return new LoginResponse(user.Id, user.Email, user.UserName, token);
		}
	}
}
