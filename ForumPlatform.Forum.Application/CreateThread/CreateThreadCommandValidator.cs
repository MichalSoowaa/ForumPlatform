using FluentValidation;

namespace ForumPlatform.Forum.Application.CreateThread
{
	public class CreateThreadCommandValidator : AbstractValidator<CreateThreadCommand>
	{
		public CreateThreadCommandValidator()
		{
			RuleFor(x => x.Title)
			.NotEmpty().WithMessage("Thread title is required.")
			.MaximumLength(300).WithMessage("Title must be at most 300 characters.");

			RuleFor(x => x.Body)
				.NotEmpty().WithMessage("Thread body is required.");

			RuleFor(x => x.SubforumId)
				.NotEmpty().WithMessage("SubforumId is required.");

			RuleFor(x => x.AuthorId)
				.NotEmpty().WithMessage("AuthorId is required.");
		}
	}
}
