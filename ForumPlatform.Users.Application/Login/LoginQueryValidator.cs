using FluentValidation;

namespace ForumPlatform.Users.Application.Login
{
	public class LoginQueryValidator : AbstractValidator<LoginQuery>
	{
		public LoginQueryValidator()
		{
			RuleFor(x => x.Email)
				.NotEmpty().WithMessage("Email is required.")
				.EmailAddress().WithMessage("Email must be a valid email address");

			RuleFor(x => x.Password)
				.NotEmpty().WithMessage("Password is required");
		}
	}
}
