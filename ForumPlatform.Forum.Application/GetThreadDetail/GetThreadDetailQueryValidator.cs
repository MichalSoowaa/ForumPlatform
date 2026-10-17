using FluentValidation;

namespace ForumPlatform.Forum.Application.GetThreadDetail
{
	public class GetThreadDetailQueryValidator : AbstractValidator<GetThreadDetailQuery>
	{
		public GetThreadDetailQueryValidator()
		{
			RuleFor(q => q.Sort).IsInEnum().WithMessage("Sort mode must be a valid value.");
		}
	}
}
