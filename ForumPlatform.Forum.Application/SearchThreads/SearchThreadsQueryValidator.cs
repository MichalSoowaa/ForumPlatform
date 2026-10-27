using FluentValidation;

namespace ForumPlatform.Forum.Application.SearchThreads
{
	public class SearchThreadsQueryValidator : AbstractValidator<SearchThreadsQuery>
	{
		public SearchThreadsQueryValidator()
		{
			RuleFor(x => x.Text)
			.NotEmpty().WithMessage("Enter something to search for.")
			.MinimumLength(2).WithMessage("Search text must be at least 2 characters.")
			.MaximumLength(200).WithMessage("Search text must be at most 200 characters.");
		}
	}
}
