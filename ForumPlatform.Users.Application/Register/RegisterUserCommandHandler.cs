using ForumPlatform.Users.Application.Abstraction;
using ForumPlatform.Users.Domain.Entities.User;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ForumPlatform.Users.Application.Register
{
	public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserResponse>
	{
		private readonly UserManager<IdentityUser<Guid>> _userManager;
		private readonly ITokenService _tokenService;

		public RegisterUserCommandHandler(UserManager<IdentityUser<Guid>> userManager, ITokenService tokenService)
		{
			_userManager = userManager;
			_tokenService = tokenService;
		}

		public async Task<RegisterUserResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
		{
			var existingUser = await _userManager.FindByEmailAsync(request.Email);

			if(existingUser != null)
			{
				throw new Exception($"User with email {request.Email} already exists.");
			}

			var passwordHasher = new PasswordHasher<IdentityUser<Guid>>();
			var dummyUser = new IdentityUser<Guid>();
			var passwordHash = passwordHasher.HashPassword(dummyUser, request.Password);

			var user = User.Create(request.Email, request.Username, passwordHash);

			var identityUser = new IdentityUser<Guid>
			{
				Id = user.Id,
				UserName = user.Username,
				Email = user.Email,
				EmailConfirmed = false
			};

			var result = await _userManager.CreateAsync(identityUser, request.Password);

			if (!result.Succeeded)
			{
				var errors = string.Join(", ", result.Errors.Select(e => e.Description));
				throw new Exception($"Failed to create user: {errors}");
			}

			var token = _tokenService.GenerateToken(user.Id, user.Email, user.Username);

			return new RegisterUserResponse(user.Id, user.Email, user.Username, token);
		}
	}
}
