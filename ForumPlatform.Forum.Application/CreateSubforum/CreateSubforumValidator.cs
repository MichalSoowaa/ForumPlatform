using FluentValidation;

namespace ForumPlatform.Forum.Application.CreateSubforum
{
	public class CreateSubforumValidator : AbstractValidator<CreateSubforumCommand>
	{
		public CreateSubforumValidator() 
		{
			RuleFor(x => x.Name)
				.NotEmpty().WithMessage("Subforum name is required.")
				.Length(3, 100).WithMessage("Subforum name must be between 3 and 100 characters.");

			RuleFor(x => x.Description)
			.NotEmpty().WithMessage("Subforum description is required.")
			.MaximumLength(1000).WithMessage("Description must be at most 1000 characters.");

			RuleFor(x => x.CreatedByUserId)
				.NotEmpty().WithMessage("CreatedByUserId is required.");
		}
	}
}
