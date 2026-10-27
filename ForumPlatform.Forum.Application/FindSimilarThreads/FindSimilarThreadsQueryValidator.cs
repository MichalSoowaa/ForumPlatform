using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumPlatform.Forum.Application.FindSimilarThreads
{
	public class FindSimilarThreadsQueryValidator : AbstractValidator<FindSimilarThreadsQuery>
	{
		public FindSimilarThreadsQueryValidator()
		{
			RuleFor(x => x.Title).NotEmpty().MaximumLength(300);
			// Bounds the size of each embedding request. The client only needs to send the start of the body.
			RuleFor(x => x.Body).MaximumLength(5000);
		}
	}
}
