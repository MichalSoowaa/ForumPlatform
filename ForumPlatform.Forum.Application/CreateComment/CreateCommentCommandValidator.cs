using FluentValidation;

namespace ForumPlatform.Forum.Application.CreateComment
{
	public class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
	{
		public CreateCommentCommandValidator()
		{
			RuleFor(x => x.Body)
				.NotEmpty().WithMessage("Comment body is required.");

			RuleFor(x => x.ThreadId)
				.NotEmpty().WithMessage("ThreadId is required.");

			RuleFor(x => x.AuthorId)
				.NotEmpty().WithMessage("AuthorId is required.");
		}
	}
}
