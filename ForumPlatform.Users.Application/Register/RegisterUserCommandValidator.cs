using FluentValidation;

namespace ForumPlatform.Users.Application.Register
{
	public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
	{
		public RegisterUserCommandValidator()
		{
			RuleFor(x => x.Email)
				.NotEmpty().WithMessage("Email is required.")
				.EmailAddress().WithMessage("Invalid email format.");
			RuleFor(x => x.Username)
				.NotEmpty().WithMessage("Username is required.")
				.Length(3, 50).WithMessage("Username must be between 3 and 50 characters long.");
			RuleFor(x => x.Password)
				.NotEmpty().WithMessage("Password is required.")
				.Length(8, 100).WithMessage("Password must be at least 8 characters long.");
			RuleFor(x => x.PasswordConfirm)
				.Equal(x => x.Password).WithMessage("Passwords do not match.");
		}
	}
}
