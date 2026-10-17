using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumPlatform.Forum.Application.GetThreadsBySubforum
{
	public class GetThreadsBySubforumQueryValidator : AbstractValidator<GetThreadsBySubforumQuery>
	{
		public GetThreadsBySubforumQueryValidator()
		{
			RuleFor(q => q.Sort).IsInEnum().WithMessage("Sort mode must be a valid value.");
		}
	}
}
